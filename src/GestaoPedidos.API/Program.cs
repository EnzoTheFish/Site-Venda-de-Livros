using GestaoPedidos.API.Clients;
using GestaoPedidos.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Adiciona o gerador Swagger

builder.Services.AddHttpClient<ICatalogClient, CatalogClient>(client =>
{
    client.BaseAddress = new Uri("http://catalog.api:8080"); // porta do Catalog.API
});

builder.Services.AddScoped<PedidoService>();

var app = builder.Build();



        app.UseSwagger(); // Habilita o middleware Swagger
        app.UseSwaggerUI(); // Habilita o middleware SwaggerUI


        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();