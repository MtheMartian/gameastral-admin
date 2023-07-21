using System.Web;

namespace GameStarBackend.Config
{
    public class Links
    {
        public const string gsAdminPage = "https://gsbackend.herokuapp.com";


        static public async Task<string> GetRawUrl(HttpContext ctx)
        {
            ctx.Request.EnableBuffering();
            ctx.Request.Body.Position = 0;
            var rawRequestBody = await new StreamReader(ctx.Request.Body).ReadToEndAsync();
            string newString = HttpUtility.UrlDecode(rawRequestBody);
            return newString;

        }
    }
}
