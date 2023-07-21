using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using GameStarBackend.Config;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json;
using System.Web;

namespace GameStarBackend.Api.Config
{
    public class AuthProperties
    {
        public async Task<AuthenticateResult> IsLoggedIn(HttpContext ctx)
        {
            var result = await ctx.AuthenticateAsync("default");
            if (result.Succeeded)
            {
                ctx.Response.Headers.Add("Cache-Control", 
                    "no-cache, no-store, must-revalidate, max-age=0");
            }
            return result;
        }

        public ActionResult ChallengeIt()
        {
            return new RedirectResult("/signin", false);
        }

        public ActionResult ReturnUserToMain()
        {
            return new RedirectResult("/", false);
        }
    }
}
