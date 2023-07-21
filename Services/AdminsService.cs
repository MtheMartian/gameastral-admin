using MongoDB.Driver;
using GameStarBackend.Api.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;

namespace GameStarBackend.Api.Services
{
    public class AdminsService
    {
        private readonly IMongoCollection<Admin> _adminsCollection;

        public AdminsService(IOptions<GameInfoDatabaseSettings> gameInfoDatabaseSettings)
        {
            var mongoClient = new MongoClient(gameInfoDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(gameInfoDatabaseSettings.Value.DatabaseName);
            _adminsCollection = mongoDatabase.GetCollection<Admin>(gameInfoDatabaseSettings.Value.AdminsCollectionName);
        }

        public async Task<List<Admin>> GetAdmins()
        {
            return await _adminsCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Admin> GetSpecificAdmin(Admin admin)
        {
            return await _adminsCollection.Find(x => x.Email == admin.Email).FirstOrDefaultAsync();
        }

        public async Task<Admin> GetSpecificAdmin(string id)
        {
           return await _adminsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Admin> SignUp(Admin user)
        {
            await _adminsCollection.InsertOneAsync(user);
            return await GetSpecificAdmin(user);
        }

        public async Task<Admin?> SignIn(Admin user)
        {
           return await _adminsCollection.Find(x => x.Email == user.Email).FirstOrDefaultAsync();
        }
    }
}
