using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Stock.API.Models
{
    public class Stock
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonId]
        [BsonElement(Order = 0)]   // Custom order
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        [BsonElement(Order = 1)]   // Custom order
        public int ProductId { get; set; }

        [BsonRepresentation(BsonType.Int64)]
        [BsonElement(Order = 2)]   // Custom order
        public int Count { get; set; }

    }
}
