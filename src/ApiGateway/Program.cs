using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(context =>
    {
        var prefix = context.Route.RouteId switch
        {
            "catalogRoute" => "/catalog",
            "pedidoRoute" => "/pedidos",
            _ => null
        };

        if (prefix is null)
            return;

        context.AddRequestTransform(transformContext =>
        {
            transformContext.ProxyRequest.Headers.Remove("X-Forwarded-Prefix");
            transformContext.ProxyRequest.Headers.TryAddWithoutValidation("X-Forwarded-Prefix", prefix);

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

app.MapReverseProxy();

app.Run();
