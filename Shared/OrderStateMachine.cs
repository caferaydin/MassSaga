using MassTransit;

namespace Shared
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    public State Submitted { get; private set; }
    public State Processing { get; private set; }
    public State Completed { get; private set; }

    public Event<OrderSubmitted> OrderSubmitted { get; private set; }
    public Event<OrderProcessed> OrderProcessed { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderProcessed, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context => Console.WriteLine($"Order Submitted: {context.Data.OrderId}"))
                .TransitionTo(Processing));

        During(Processing,
            When(OrderProcessed)
                .Then(context => Console.WriteLine($"Order Processed: {context.Data.OrderId}"))
                .TransitionTo(Completed)
                .Finalize());
    }
}
}
