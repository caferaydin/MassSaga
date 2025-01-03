using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.Contexts;
using SagaStateMachine.Service.StateInstances;
using SagaStateMachine.Service.StateMachines;

var builder = Host.CreateApplicationBuilder(args);



#region Configuration

var massTransitSettings = builder.Configuration.GetSection("MassTransitSettings");

builder.Services.AddMassTransit(conf =>
{

    conf.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
        .EntityFrameworkRepository(option =>
        {
            //option.ConcurrencyMode = ConcurrencyMode.Pessimistic; // or use Optimistic, which requires RowVersion
            option.AddDbContext<DbContext, OrderStateDbContext>((provider, _builder) =>
            {
                _builder.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
        });

    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(massTransitSettings["Host"], massTransitSettings["VirtualHost"], h =>
        {
            h.Username(massTransitSettings["UserName"]);
            h.Password(massTransitSettings["Password"]);
        });
    });

});

#endregion

var host = builder.Build();
host.Run();