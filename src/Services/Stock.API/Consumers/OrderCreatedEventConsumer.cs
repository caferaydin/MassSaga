using MassTransit;
using Shared.OrderEvents;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(MongoDbService mongodbService) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            List<Models.Stock> stockCollection = (List<Models.Stock>)mongodbService.GetCollection<Stock.API.Models.Stock>();

            foreach (var orderItem in context.Message.OrterItems)
            {
                
            }
        }
    }
}
