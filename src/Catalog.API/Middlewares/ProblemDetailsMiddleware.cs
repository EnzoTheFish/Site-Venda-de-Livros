    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.Net;
    using System.Text.Json;

     namespace Catalog.API.Middlewares{
    public class ProblemDetailsMiddleware {
        private readonly RequestDelegate _next;
        public ProblemDetailsMiddleware(RequestDelegate next) {
                _next = next;
        }
    public async Task InvokeAsync(HttpContext context) {
        try{ 
            await _next(context);
        }
        catch (Exception ex) {
            await HandleExceptionAsync(context, ex);
        }
    }
    private static Task HandleExceptionAsync(HttpContext context, Exception exception){
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var problemDetails = new ProblemDetails {
            Status = (int)HttpStatusCode.InternalServerError,
            Type = "https://tools.ietf.org/html/rfc7807#section-6.6.1", //Exemplo de URI para Internal Server Error
            Title = "Ocorreu um erro interno no servidor.",
            Detail = exception.Message,
            Instance = context.Request.Path
        }; // Adicionar detalhes específicos para ambientes de desenvolvimento
            if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() ==true){
                problemDetails.Extensions.Add("traceId",
                System.Diagnostics.Activity.Current?.Id ?? context.TraceIdentifier);
                problemDetails.Extensions.Add("stackTrace", exception.StackTrace);
            }
                return
                context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
    }
}
