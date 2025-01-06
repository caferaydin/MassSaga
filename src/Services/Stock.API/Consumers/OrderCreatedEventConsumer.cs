using MassTransit;
using MongoDB.Driver;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider) : IConsumer<OrderCreatedEvent>
    {

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResults = new();
            var stockCollection = mongoDbService.GetCollection<Stock.API.Models.Stock>();

            foreach (var orderItem in context.Message.OrterItems)
            {
                var filter = Builders<Stock.API.Models.Stock>.Filter.Where(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count);
                stockResults.Add(await (await stockCollection.FindAsync(filter)).AnyAsync());
            }


            var sendEndPoint = await sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettings.StateMachineQueue}"));


            if (stockResults.TrueForAll(s => s.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrterItems)
                {
                    var stocks = await (await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stocks.Count -= orderItem.Count;

                    await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stocks);
                }

                StockReservedEvent stockReservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrterItems
                };
                await sendEndPoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    Message = "Stock yetersiz"
                };

                await sendEndPoint.Send(stockNotReservedEvent);
            }
        }
    }
}
