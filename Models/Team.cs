using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Models
{
    public class Team
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = "N/A";
        public string Position { get; set; } = "N/A";
        public string Picture { get; set; } = "N/A";
        public string Description { get; set; } = "N/A";
    }
}
