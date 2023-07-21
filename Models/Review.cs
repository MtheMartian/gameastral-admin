using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Api.Models
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? DisplayName { get; set; }
        public string? GameReview { get; set; }
        public DateTime WhenPosted { get; set; } = DateTime.Now;
        public string GameId { get; set; } = null!;
    }
}
