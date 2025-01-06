using MassTransit;
using MongoDB.Driver;
using Shared.Settings;
using Stock.API.Consumers;
using Stock.API.Models;
using Stock.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


//# region Add Configuration 
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


//var massTransitSettings = builder.Configuration.GetSection("MassTransitSettings");

//builder.Services.AddMassTransit(conf =>
//{
//    conf.UsingRabbitMq((ctx, cfg) =>
//    {
//        cfg.Host(massTransitSettings["Host"], massTransitSettings["VirtualHost"], h =>
//        {
//            h.Username(massTransitSettings["UserName"]);
//            h.Password(massTransitSettings["Password"]);
//        });
//    });
//});

//#endregion


builder.Services.AddSingleton<MongoDbService>();
using var scope = builder.Services.BuildServiceProvider().CreateScope();
var mongoDbService = scope.ServiceProvider.GetRequiredService<MongoDbService>();

if (!await (await mongoDbService.GetCollection<Stock.API.Models.Stock>().FindAsync(q => true)).AnyAsync())
{
    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock()
    {
        ProductId = 1,
        Count = 200
    });

    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock()
    {
        ProductId = 2,
        Count = 300
    });

    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock()
    {
        ProductId = 3,
        Count = 50
    });

    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock()
    {
        ProductId = 4,
        Count = 10
    });

    mongoDbService.GetCollection<Stock.API.Models.Stock>().InsertOne(new Stock.API.Models.Stock()
    {
        ProductId = 5,
        Count = 60
    });


}

var massTransitSettings = builder.Configuration.GetSection("MassTransitSettings");

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<OrderCreatedEventConsumer>();
    conf.AddConsumer<StockRollbackMessageConsumer>();
    conf.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitSettings["Host"], massTransitSettings["VirtualHost"], h =>
        {
            h.Username(massTransitSettings["UserName"]);
            h.Password(massTransitSettings["Password"]);
        });

        cfg.ReceiveEndpoint(RabbitMqSettings.Stock_OrderCreatedEventQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
        cfg.ReceiveEndpoint(RabbitMqSettings.Stock_RollbackMessageQueue, e => e.ConfigureConsumer<StockRollbackMessageConsumer>(context));
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseSwagger();
//app.UseSwaggerUI();

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

app.Run();