using GestaoPedidos.Notificacoes;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PedidoCriadoConsumer>();

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

var host = builder.Build();
host.Run();
