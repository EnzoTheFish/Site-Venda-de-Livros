    var builder = WebApplication.CreateBuilder(args); // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(); // Adiciona o gerador Swagger
    var app = builder.Build(); // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) {
            app.UseSwagger(); // Habilita o middleware Swagger
            app.UseSwaggerUI(); // Habilita o middleware SwaggerUI
        }
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();