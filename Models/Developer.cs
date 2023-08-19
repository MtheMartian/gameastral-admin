using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Models
{
    public class Developer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; } = "N/A";
        public string Logo { get; set; } = "N/A";
        public string Info { get; set; } = "N/A";
    }
}
