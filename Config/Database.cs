using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GameStarBackend.Api.Config
{
    public class Database
    {
        public void ConnectDB()
        {
            var superString = Environment.GetEnvironmentVariable("DB_STRING");
            var client = new MongoClient(superString);
            var database = client.GetDatabase("gameinfo");
            Console.WriteLine($"Connected to {database.DatabaseNamespace}");
        }
    }
}
