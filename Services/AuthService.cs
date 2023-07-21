using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using GameStarBackend.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using GameStarBackend.Api.Models;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Xml.Schema;
using GameStarBackend.Api.Services;
using GameStarBackend.Readers;
using System.Globalization;
using GameStarBackend.Templates;
using GameStarBackend.Config;

namespace GameStarBackend.Api.Services
{
    public class AuthService
    {
        private readonly IKeyManager _keyManager;
        private readonly IMongoCollection<EncKeys> _keysCollection;
        private readonly AdminsService _adminService;
       public AuthService(IKeyManager keyManager,
           IOptions<GameInfoDatabaseSettings> gameInfoDatabaseSettings, AdminsService adminsService)
        {
            _keyManager = keyManager;
            _adminService = adminsService;

            MongoClient mongoClient = new(gameInfoDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(gameInfoDatabaseSettings.Value.DatabaseName);

            _keysCollection = mongoDatabase.GetCollection<EncKeys>(gameInfoDatabaseSettings.Value.KeysCollectionName);
        }

        // Key Database Functions

        public async Task<EncKeys> GetSpecificKey(string userId)
        {
            return await _keysCollection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task RetrieveKey(Admin user)
        {
            var tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GS-Keys"));
            EncKeys retrievedKey = await GetSpecificKey(user.Id!);
            var keyFiles = Directory.EnumerateFiles($@"{tempFolder.FullName}");

            if (!keyFiles.Any())
            {
               Keys.WriteXMLKeyFile(retrievedKey);
            }
            else
            {
                foreach(string key in keyFiles)
                {
                    if (key.Contains(retrievedKey.KeyId))
                    {
                        Console.WriteLine("No need to fetch key.");
                        return;
                    }
                }
                Keys.WriteXMLKeyFile(retrievedKey);
            } 
        }

        public async Task StoreKey(Admin user)
        {
            if (await GetSpecificKey(user.Id) == null)
            {
                var keyValues = CustomReader.ReadMyXml(FolderReader.GetFileName());

                EncKeys newKey = new EncKeys()
                {
                    UserId = user.Id,
                    KeyId = keyValues["keyInfo"][0],
                    CreationDate = keyValues["keyInfo"][1],
                    ActDate = keyValues["keyInfo"][2],
                    ExpDate = keyValues["keyInfo"][3],
                    Descriptor = keyValues["descriptor"]
                };

                await _keysCollection.InsertOneAsync(newKey);
                Console.WriteLine("Key assigned!");
                return;
            }
            else
            {
                Console.WriteLine("User already has a key assigned!");
                return;
            }
        }

        public string AccountProtector(string use, string action, string entry)
        {
            var tempFolder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "GS-Keys"));
            var _serviceCollection = new ServiceCollection();

            _serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(tempFolder);
                
            var services = _serviceCollection.BuildServiceProvider();

            if(action.ToLower() == "protect")
            {
                FolderReader.CleanFolder();
                Thread.Sleep(2000);
                string protectedPass = services.GetDataProtector(use).Protect(entry);
                Console.WriteLine("Protected successfully!");
                Thread.Sleep(2000);
                return protectedPass;
            }
            else if(action.ToLower() == "unprotect")
            {
                string unprotectedPass = services.GetDataProtector(use).Unprotect(entry);
                Console.WriteLine("Unprotected successfully!");
                return unprotectedPass;
            }

            return null;
        }

        public async Task CreateCookie(HttpContext ctx)
        {
            await ctx.SignInAsync("default", new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
                    },
                    "default"
                    )
                ),
                new AuthenticationProperties()
                {
                    IsPersistent = true,
                    AllowRefresh = true,
                });
        }

        public void SetSession(HttpContext ctx, Admin admin)
        {
            ctx.Session.SetString("session", admin.Id!);
        }

        public string GetSessionId(HttpContext ctx)
        {  
            try
            {
                var idFromSession = ctx.Session.GetString("session");
                return idFromSession!;
            }
            catch
            {
                ArgumentNullException nothing = new();
                throw new Exception("Session Id is empty.", nothing);
            }
        }

        public async Task DeleteCookies(HttpContext ctx)
        {
            await ctx.SignOutAsync("default");
            ctx.Session.Clear();
            ctx.Response.Cookies.Delete("_GSAdmin_session");
            ctx.Response.Redirect($"{Links.gsAdminPage}/signin");
        }
    }
}
