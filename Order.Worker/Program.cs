using MassTransit;
using MongoDB.Driver;
using Shared;

var builder = Host.CreateApplicationBuilder(args);



builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .MongoDbRepository(r =>
        {
            r.Connection = "mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+2.3.4";
            r.DatabaseName = "worker";
        });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// MongoDB'ye baðlantý kuruyoruz
builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://127.0.0.1:27017/?directConnection=true&serverSelectionTimeoutMS=2000&appName=mongosh+2.3.4"));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("worker"); // MongoDB'deki "worker" veritabanýna baðlanýyoruz.
});



//builder.Services.AddHostedService<Worker>();


var host = builder.Build();
host.Run();
