namespace GameStarBackend.Api.Models
{
    public class CloudinaryResults
    {
        public string public_id { get; set; } = null!;
        public string secure_url { get; set; } = null!;

        public string? result { get; set; }
    }
}
