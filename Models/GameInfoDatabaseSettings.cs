namespace GameStarBackend.Api.Models
{
    public class GameInfoDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string GamesCollectionName { get; set; } = null!;
        public string ReviewsCollectionName { get; set; } = null!;
        public string AdminsCollectionName { get; set; } = null!;
        public string KeysCollectionName { get; set; } = null!;
    }
}
