using MassTransit;
using Payment.API.Consumers;
using Shared.Settings;

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



var massTransitSettings = builder.Configuration.GetSection("MassTransitSettings");

builder.Services.AddMassTransit(conf =>
{
    conf.AddConsumer<PaymentStartedEventConsumer>();
    conf.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(massTransitSettings["Host"], massTransitSettings["VirtualHost"], h =>
        {
            h.Username(massTransitSettings["UserName"]);
            h.Password(massTransitSettings["Password"]);
        });

        cfg.ReceiveEndpoint(RabbitMqSettings.Payment_StartedEventQueue, e => e.ConfigureConsumer<PaymentStartedEventConsumer>(context)); 
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