using MongoDB.Driver;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GameStarBackend.Models;
using Microsoft.Extensions.Options;
using GameStarBackend.Api.Models;

namespace GameStarBackend.Services
{
    public class DevelopersService
    {
        private readonly IMongoCollection<Developer> _devsCollection;
        private readonly Cloudinary cloudinary;
        private ResourceType image;

        public DevelopersService(IOptions<GameInfoDatabaseSettings> dbs,
                                  IConfiguration cloudinaryGSSettings)
        {
            var mongoClient = new MongoClient(dbs.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbs.Value.DatabaseName);

            _devsCollection = mongoDatabase.GetCollection<Developer>(dbs.Value.DevelopersCollectionName);

            cloudinary = new Cloudinary(cloudinaryGSSettings["CloudinaryUrl"]);
        }
    }
}
