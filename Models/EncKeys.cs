using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Models
{
    public class EncKeys
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string UserId { get; set; } = null!;
        public string KeyId { get; set; } = null!;
        public string CreationDate { get; set; } = null!;
        public string ActDate { get; set; } = null!;
        public string ExpDate { get; set; } = null!;
        public string[] Descriptor { get; set; } = null!;
    }
}
