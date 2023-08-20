using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using GameStarBackend.Api.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Sprache;
using System.Text.Json;
using GameStarBackend.Models;
using GameStarBackend.Services;
using MongoDB.Driver;

namespace GameStarBackend.Api.Services
{
    public class CloudinaryService
    {
        private readonly Cloudinary cloudinary;
        private readonly IMongoCollection<Game> _gameCollection;
        private readonly IMongoCollection<Team> _teamCollection;
        private readonly IMongoCollection<Developer> _developerCollection;

        private readonly IConfiguration Configuration;


        public CloudinaryService(IConfiguration configuration,
                                 IOptions<GameInfoDatabaseSettings> dbs)
        {
            CloudinaryGSSettings cloudinarySettings = new CloudinaryGSSettings();
            Configuration = configuration;
            Configuration.GetSection(CloudinaryGSSettings.CloudinaryGS).Bind(cloudinarySettings);
            cloudinary = new Cloudinary(cloudinarySettings.CloudinaryUrl);

            var mongoClient = new MongoClient(dbs.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbs.Value.DatabaseName);

            _gameCollection = mongoDatabase.GetCollection<Game>(dbs.Value.GamesCollectionName);
            _teamCollection = mongoDatabase.GetCollection<Team>(dbs.Value.TeamCollectionName);
            _developerCollection = mongoDatabase.GetCollection<Developer>(dbs.Value.DevelopersCollectionName);
        }

        private async Task<string> GetFilesFromRequest(HttpContext ctx)
        {
            FileInfo fileInfo;
            var files = ctx.Request.Form.Files;
            var imgFile = files[0];
            fileInfo = new FileInfo(imgFile.FileName);
            if (fileInfo.Extension == ".png" || fileInfo.Extension == ".jpg" ||
                fileInfo.Extension == ".webp" || fileInfo.Extension == ".jpeg")
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imgFile.CopyToAsync(stream);
                }

                return filePath;
            }
            return null;
        }

        private async Task<string[]> UploadImage(HttpContext ctx)
        {
            string[] cloudinaryResults = new string[2];
            string fileInfo = await GetFilesFromRequest(ctx);
            cloudinary.Api.Secure = true;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileInfo),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
            var result = JsonSerializer.Deserialize<CloudinaryResults>(uploadResult.JsonObj.ToString());

            cloudinaryResults[0] = result.public_id;
            cloudinaryResults[1] = result.secure_url;

            return cloudinaryResults;
        }

        public async Task ReplaceImage(HttpContext ctx, string id, string model)
        {
            Game? game;
            Team? team;
            Developer? dev;
            string[] result;

            switch (model)
            {
                case "game":
                    game = await _gameCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

                    if(game.CloudinaryId != String.Empty)
                    {
                        var deleteParams = new DeletionParams(game.CloudinaryId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var response = await cloudinary.DestroyAsync(deleteParams);
                        var res = JsonSerializer.Deserialize<CloudinaryResults>(response.JsonObj.ToString());

                        if(res.result != "ok")
                        {
                            ctx.Response.Headers.Add("imageResult", "fail");
                            return;
                        }
                    }

                    result = await this.UploadImage(ctx);
                    game.CloudinaryId = result[0];
                    game.ImgURL = result[1];
                    await _gameCollection.ReplaceOneAsync(x => x.Id == id, game);

                    break;

                case "team":
                    team = await _teamCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

                    if (team.CloudinaryId != String.Empty)
                    {
                        var deleteParams = new DeletionParams(team.CloudinaryId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var response = await cloudinary.DestroyAsync(deleteParams);
                        var res = JsonSerializer.Deserialize<CloudinaryResults>(response.JsonObj.ToString());

                        if (res.result != "ok")
                        {
                            ctx.Response.Headers.Add("imageResult", "fail");
                            return;
                        }
                    }

                    result = await this.UploadImage(ctx);
                    team.CloudinaryId = result[0];
                    team.Picture = result[1];
                    await _teamCollection.ReplaceOneAsync(x => x.Id == id, team);

                    break;

                case "developer":
                    dev = await _developerCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

                    if (dev.CloudinaryId != String.Empty)
                    {
                        var deleteParams = new DeletionParams(dev.CloudinaryId)
                        {
                            ResourceType = ResourceType.Image
                        };

                        var response = await cloudinary.DestroyAsync(deleteParams);
                        var res = JsonSerializer.Deserialize<CloudinaryResults>(response.JsonObj.ToString());

                        if (res.result != "ok")
                        {
                            ctx.Response.Headers.Add("imageResult", "fail");
                            return;
                        }
                    }

                    result = await this.UploadImage(ctx);
                    dev.CloudinaryId = result[0];
                    dev.Logo = result[1];
                    await _developerCollection.ReplaceOneAsync(x => x.Id == id, dev);

                    break;

                case "publisher":
                    break;
            }
        }
    }
}
