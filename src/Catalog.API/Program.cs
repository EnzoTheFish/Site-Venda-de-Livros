    using Catalog.API.Services;
    using Microsoft.OpenApi.Models;

    var builder = WebApplication.CreateBuilder(args); // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(); // Adiciona o gerador Swagger
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddSingleton<IProductStore, InMemoryProductStore>();
        builder.Services.AddScoped<IProdutoCacheService, ProdutoCacheService>();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
            options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "catalog:";
        });
    var app = builder.Build(); // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
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
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();

public partial class Program
{
}
