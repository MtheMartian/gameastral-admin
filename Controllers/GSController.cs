using Microsoft.AspNetCore.Mvc;
using GameStarBackend.Config;
using Microsoft.AspNetCore.Cors;

namespace GameStarBackend.Controllers
{
    [ApiController]
    [Route("/")]
    [Route("/signin")]
    [Route("/signup")]
    public class GSController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public GSController(IWebHostEnvironment environment)
        {
            _webHostEnvironment = environment;
        }

        [EnableCors("_allowedOrigins")]
        [HttpGet]
        public ActionResult RenderPageOrConf()
        {
            if (HttpContext.Request.Headers.Origin.Equals("http://localhost:3000"))
            {
                return Ok();
            }
            var path = Path.Combine(_webHostEnvironment.WebRootPath, "Views", "index.html");
            var fileStream = System.IO.File.OpenRead(path);
            Response.Headers.Add("Alive", "awake");
            return File(fileStream, "text/html");
        }
    }
}
