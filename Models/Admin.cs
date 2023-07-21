using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Api.Models
{
    public class Admin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
