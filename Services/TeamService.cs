using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MongoDB.Driver;
using GameStarBackend.Models;
using Microsoft.Extensions.Options;
using GameStarBackend.Api.Models;
using System.ComponentModel;

namespace GameStarBackend.Services
{
    public class TeamService
    {
        private readonly IMongoCollection<Team> _teamCollection;

        public TeamService(IOptions<GameInfoDatabaseSettings> dbs)
        {
            var mongoClient = new MongoClient(dbs.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(dbs.Value.DatabaseName);

            _teamCollection = mongoDatabase.GetCollection<Team>(dbs.Value.TeamCollectionName);
        }

        public async Task<List<Team>> GetAsync()
        {
            return await _teamCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Team> GetAsync(string id)
        {
            return await _teamCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Team>> GetAsync(string[] ids)
        {
            List<Team> teamMembers = new List<Team>();

            foreach(string idItem in ids)
            {
                teamMembers.Add(await _teamCollection.Find(x => x.Id == idItem).FirstOrDefaultAsync());
            }

            return teamMembers;
        }

        public async Task CreateAsync(Team team)
        {
            await _teamCollection.InsertOneAsync(team);
        }

        public async Task UpdateAsync(Team team, string id)
        {
            await _teamCollection.ReplaceOneAsync(x => x.Id == id, team);
        }

        public async Task RemoveAsync(string id)
        {
            await _teamCollection.DeleteOneAsync(x => x.Id == id);
        }
    }
}
