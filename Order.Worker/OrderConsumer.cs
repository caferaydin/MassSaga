using MassTransit;
using MongoDB.Driver;
using Shared;

namespace Order.Worker
{
    public class OrderConsumer : IConsumer<OrderSubmitted>
    {
        private readonly ILogger<OrderConsumer> _logger;
        private readonly IMongoCollection<Shared.Order> _orderCollection;

        public OrderConsumer(IMongoCollection<Shared.Order> orderCollection, ILogger<OrderConsumer> logger)
        {
            _orderCollection = orderCollection;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderSubmitted> context)
        {

            // Simulasyon: Hata durumunu tetikleyelim
            //if (context.Message.OrderId == Guid.Empty)
            //{
            //    _logger.LogError("Order ID is empty. Failing the process.");

            //    // Hata durumunu bildir
            //    await context.Publish(new OrderFailed
            //    {
            //        OrderId = context.Message.OrderId,
            //        Reason = "Invalid Order ID"
            //    });
            //    return;
            //}

            //// Burada işlem başarılı olursa:
            //await context.Publish(new OrderProcessed
            //{
            //    OrderId = context.Message.OrderId
            //});

            var order = new Shared.Order
            {
                OrderId = context.Message.OrderId,
                Status = "Processing",
                OrderDate = context.Message.OrderDate
            };

            // MongoDB'ye kaydet
            await _orderCollection.InsertOneAsync(order);

            // İşlem başarılıysa, OrderProcessed mesajını gönder
            await context.Publish(new OrderProcessed
            {
                OrderId = context.Message.OrderId
            });
        }
    }
}
