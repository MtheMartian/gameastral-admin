using GameStarBackend.Api.Config;
using GameStarBackend.Api.Services;
using GameStarBackend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using GameStarBackend.Models;
using System.Text.Json;
using System.Web;

namespace GameStarBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _teamService;
        private readonly AuthProperties _authProps;
        private readonly CloudinaryService _cloudinaryService;

        public TeamController(TeamService teamService, AuthProperties authProps,
                                CloudinaryService cloudinaryService)
        {
            _teamService = teamService;
            _authProps = authProps;
            _cloudinaryService = cloudinaryService;
        }

        private async Task<string> ReturnBodyRequest()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var rqBody = await new StreamReader(Request.Body).ReadToEndAsync();
            string decodedBody = HttpUtility.UrlDecode(rqBody);

            return decodedBody;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet]
        public async Task<List<Team>> Get()
        {
            return await _teamService.GetAsync();
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("{id}")]
        public async Task<Team> Get(string id)
        {
            return await _teamService.GetAsync(id);
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet("specific")]
        public async Task<List<Team>> GetSpecificMembers()
        {
            var decodedBody = await this.ReturnBodyRequest();
            string[]? ids = JsonSerializer.Deserialize<string[]>(decodedBody);
            return await _teamService.GetAsync(ids);
        }

        [EnableCors("_allowedOrigins")]
        [HttpPost]
        public async Task<ActionResult<string>> CreateMember()
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);

            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}

            var rqBody = await this.ReturnBodyRequest();
            Team? team = JsonSerializer.Deserialize<Team>(rqBody);
            
            if(team == null)
            {
                return "Fail";
            }

            await _teamService.CreateAsync(team);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateMember(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);

            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}

            var rqBody = await this.ReturnBodyRequest();
            Team? team = JsonSerializer.Deserialize<Team>(rqBody);

            if (team == null)
            {
                return "Fail";
            }

            team.Id = id;
            await _teamService.UpdateAsync(team, id);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteMember(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);

            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}
            var team = await _teamService.GetAsync(id);

            var result = await _cloudinaryService.DeleteImage(team);

            if(result == null || result != "ok")
            {
                return BadRequest(result);
            }

            await _teamService.RemoveAsync(id);
            return Ok();
        }

        [HttpPost("upload/{id}")]
        public async Task<ActionResult> UploadPicture(string id)
        {
            //var result = await _authProps.IsLoggedIn(HttpContext);

            //if (!result.Succeeded)
            //{
            //    return _authProps.ChallengeIt();
            //}

            await _cloudinaryService.ReplaceImage(HttpContext, id, "team");
            return Ok();
        }
    }
}
