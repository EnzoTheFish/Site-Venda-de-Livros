using GestaoPedidos.API.Clients;
using GestaoPedidos.API.Services;
using MassTransit;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Adiciona o gerador Swagger

builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["CatalogApi:BaseUrl"] ?? "http://catalog.api:8080");
});

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var section = builder.Configuration.GetSection("RabbitMq");
        var host = section["Host"] ?? "localhost";
        var port = ushort.TryParse(section["Port"], out var configuredPort) ? configuredPort : (ushort)5672;
        var username = section["Username"] ?? "guest";
        var password = section["Password"] ?? "guest";

        cfg.Host(host, port, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<PedidoService>();

var app = builder.Build();



        app.UseSwagger(options => // Habilita o middleware Swagger
        {
            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                var prefix = httpReq.Headers["X-Forwarded-Prefix"].FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(prefix))
                    swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = prefix } };
            });
        });
        app.UseSwaggerUI(); // Habilita o middleware SwaggerUI


        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();

public partial class Program
{
}
