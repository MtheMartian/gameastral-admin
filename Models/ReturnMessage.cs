using GameStarBackend.Api.Models;

namespace GameStarBackend.Api.Models
{
    public class ReturnMessage
    {
        public string Message { get; set; } = null!;
        public List<Review>? Data { get; set; }
    }
}
