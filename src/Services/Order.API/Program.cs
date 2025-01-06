using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Order.API.Consumers;
using Order.API.Contexts;
using Order.API.Models;
using Order.API.ViewModels;
using Shared.Messages;
using Shared.OrderEvents;
using Shared.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

# region Add Configuration 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

var massTransitSettings = builder.Configuration.GetSection("MassTransitSettings");

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<OrderCompletedEventConsumer>();
    conf.AddConsumer<OrderFailedEventConsumer>();

    conf.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitSettings["Host"], massTransitSettings["VirtualHost"], h =>
        {
            h.Username(massTransitSettings["UserName"]);
            h.Password(massTransitSettings["Password"]);
        });

        cfg.ReceiveEndpoint(RabbitMqSettings.Order_OrderCompletedEventQueue, e => e.ConfigureConsumer<OrderCompletedEventConsumer>(context));
        cfg.ReceiveEndpoint(RabbitMqSettings.Order_OrderFailedEventQueue, e => e.ConfigureConsumer<OrderFailedEventConsumer>(context));
    });
});

#endregion




var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();


app.MapPost("/create-order", async (CreateOrderVM model, OrderDbContext context, ISendEndpointProvider sendEndpointProvider) =>
{
    var order = new Order.API.Models.Order
    {
        BuyerId = model.BuyerId,
        CreatedDate = DateTime.UtcNow,
        OrderStatus = Order.API.Enum.OrderStatus.Suspend,
        TotalPrice = model.OrderItems.Sum(x => x.Price * x.Count),
        OrderItems = model.OrderItems.Select(x => new OrderItem
        {
            Count = x.Count,
            Price = x.Count,
            ProductId = x.ProductId
        }).ToList()
    };

    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();

    OrderStarterEvent orderStarterEvent = new()
    {
        OrderId = order.Id,
        BuyerId = order.BuyerId,
        TotalPrice = model.OrderItems.Sum(x => x.Price * x.Count),
        OrderItems = model.OrderItems.Select(x => new OrderItemMessage
        {
            ProductId = x.ProductId,
            Price = x.Price,
            Count = x.Count
        }).ToList()
    };

    var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{Shared.Settings.RabbitMqSettings.StateMachineQueue}"));

    await sendEndpoint.Send(orderStarterEvent);

    return Results.Created($"/order/{order.Id}", order);
});


//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();


app.Run();
