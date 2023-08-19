using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MongoDB.Driver;
using GameStarBackend.Models;
using Microsoft.Extensions.Options;
using GameStarBackend.Api.Models;

namespace GameStarBackend.Services
{
    public class TeamService
    {
        private readonly IMongoCollection<Team> _teamCollection;
        private readonly Cloudinary cloudinary;
        private ResourceType image;

        public TeamService(IOptions<GameInfoDatabaseSettings> dbs, 
                            IConfiguration cloudinaryGSSettings)
        {
            var mongoClient = new MongoClient(dbs.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbs.Value.DatabaseName);

            _teamCollection = mongoDatabase.GetCollection<Team>(dbs.Value.TeamCollectionName);

            cloudinary = new Cloudinary(cloudinaryGSSettings["CloudinaryUrl"]);
        }
    }
}
