using Microsoft.AspNetCore.Mvc;
using GameStarBackend.Api.Models;
using MongoDB.Driver;
using GameStarBackend.Api.Services;
using GameStarBackend.Api.Config;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Text;
using DnsClient.Internal;
using System.Threading;
using System.Collections;
using System.Web;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using GameStarBackend.Config;
using Microsoft.AspNetCore.Cors;

namespace GameStarBackend.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GamesService _gamesService;
        private readonly AuthProperties _authProps;
        private readonly CloudinaryService _cloudinaryService;
        
        public GamesController(GamesService gamesService, AuthProperties authProps,
                                CloudinaryService cloudinaryService)
        {
            _gamesService = gamesService; 
            _authProps = authProps;
            _cloudinaryService = cloudinaryService;
        }
            
        public async Task<Game> GetRequestBody()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var rawRequestBody = await new StreamReader(Request.Body).ReadToEndAsync();
            string newString = HttpUtility.UrlDecode(rawRequestBody);
            var gameObject = JsonSerializer.Deserialize<Game>(newString);
            return gameObject!;
        }

        public async Task<string> GetFilesFromRequest()
        {
            FileInfo fileInfo;
            var files = Request.Form.Files;
            var imgFile = files[0];
            fileInfo = new FileInfo(imgFile.FileName);
            if(fileInfo.Extension == ".png" || fileInfo.Extension == ".jpg" || 
                fileInfo.Extension == ".webp" || fileInfo.Extension == ".jpeg")
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imgFile.CopyToAsync(stream);
                }

                Console.WriteLine(filePath);
                Console.WriteLine(fileInfo.Extension);

                return filePath;
            }
            return null;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet]
        public async Task<List<Game>> Get()
        {
            List<Game> games = await _gamesService.GetAsync();
            
            return games;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Game>> GetTitle(string id)
        {
            var game = await _gamesService.GetAsync(id);

            if(game is null)
            {
                return NotFound();
            }

            return game;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("info")]
        public async Task<ActionResult<Game>> TitleMoreInfo(string title)
        {
            var game = await _gamesService.GetAsync(title);

            if (game is null)
            {
                return NotFound();
            }

            return game;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("similar")]
        public async Task<List<Game>> GetSimilarTitles(string gameId)
        {
            List<Game> similarGames = await _gamesService.SimilarTitles(gameId);
            return similarGames;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("search")]
        public async Task<List<Game>> SearchedTitles(string entry = " ")
        {
            return await _gamesService.GetTitles(entry);
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("carousel")]
        public async Task<List<Game>> Carousel()
        {
            return await _gamesService.CarouselTitles();
        }
        
        [HttpPost("[action]")]
        public async Task<IActionResult> Create()
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);
            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();   
            //}

            var gameObject = await GetRequestBody();
            if (gameObject == null)
            {
                return NotFound();
            }
            await _gamesService.CreateAsync(gameObject);
            return Redirect(Links.gsAdminPage);

        }
     
        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);
            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}
            await _gamesService.RemoveAsync(id);
            return Redirect(Links.gsAdminPage);

        }

        [EnableCors("_allowedOrigins")]
        [HttpPost("[action]/{id}")]
        public async Task<IActionResult> Upload(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);
            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}
            await _cloudinaryService.ReplaceImage(HttpContext, id, "game");
            return Redirect(Links.gsAdminPage);
        }

        [HttpPut("[action]/cat/{id}")]
        public async Task<IActionResult> Update(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);
            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}
            var newGame = await GetRequestBody();

            newGame.Id = id;

            await _gamesService.UpdateAsync(id, newGame);

            return Redirect(Links.gsAdminPage);
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("genre")]
        public async Task<List<string>> GetGenres(string entry = " ")
        {
            return await _gamesService.GetGenres(entry);
        }
    }
}
