using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStarBackend.Api.Models
{
    public class Game
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string GameTitle { get; set; } = null!;
        public string CloudinaryId { get; set; } = "";
        public string[] Tags { get; set; } = { "" };
        public string[] Platforms { get; set; } = { "" };
        public string? ImgURL { get; set; }
        public string VideoURL { get; set; } = "";
        public string GameplayVid { get; set; } = "";
        public string Summary { get; set; } = "";
        public string Publisher { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
        public string[] TeamMembers { get; set; } = { "" };
        public Dictionary<string, string> PcLinks { get; set; } = new Dictionary<string, string>()
        {
            ["steam"] = "",
            ["epicStore"] = "",
            ["titleWeb"] = "",
        };
        public Dictionary<string, string> ConsoleLinks { get; set; } = new Dictionary<string, string>()
        {
            ["pStore"] = "",
            ["xbox"] = "",
            ["nintendo"] = "",
            ["bestBuy"] = "",
            ["gameStop"] = "",
        };
        public int __v { get; set; } = 0;
    }
}
