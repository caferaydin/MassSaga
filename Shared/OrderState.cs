using MassTransit;

namespace Shared
{

    public class Order
    {
        public Guid OrderId { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }
        public DateTime? OrderDate { get; set; }
        public int Version { get; set; }
    }

    public class OrderSubmitted
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class OrderProcessed
    {
        public Guid OrderId { get; set; }
    }

    public class OrderFailed
    {
        public Guid OrderId { get; set; }
        public string Reason { get; set; }
    }


}
