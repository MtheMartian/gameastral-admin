namespace GameStarBackend.Api.Models
{
    public class CloudinaryGSSettings
    {
        public const string CloudinaryGS = "CloudinaryGS";
        public string CloudName { get; set; } = String.Empty;
        public string ApiKey { get; set; } = String.Empty;
        public string ApiSecret { get; set; } = String.Empty;
        public string CloudinaryUrl { get; set; } = String.Empty;
    }
}
