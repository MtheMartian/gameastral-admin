using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.DataProtection;
using GameStarBackend.Api.Models;
using GameStarBackend.Api.Services;
using GameStarBackend.Api.Config;
using System.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using GameStarBackend.Config;

namespace GameStarBackend.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminsController : Controller
    {
        private readonly AdminsService _adminsService;
        private readonly AuthService _authService;
        private readonly AuthProperties _authProps;

        public AdminsController(AdminsService adminsService,
            AuthService authService, AuthProperties authProps)
        {
            _adminsService = adminsService;
            _authService = authService;
            _authProps = authProps;
        }

        public async Task<Admin> AdminFromRequest()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var streamReader = await new StreamReader(Request.Body).ReadToEndAsync();
            string decodedString = HttpUtility.UrlDecode(streamReader);
            var newAdmin = JsonSerializer.Deserialize<Admin>(decodedString);
            return newAdmin;
        }

        public async Task<AdminConfirmation> AdminSignUpRequest()
        {
            Request.EnableBuffering();
            Request.Body.Position = 0;
            var streamReader = await new StreamReader(Request.Body).ReadToEndAsync();
            string decodedString = HttpUtility.UrlDecode(streamReader);
            var adminSignUp = JsonSerializer.Deserialize<AdminConfirmation>(decodedString);
            return adminSignUp;
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<Admin>> Auth()
        {
            string rawUrl = await Links.GetRawUrl(HttpContext);
            var result = await _authProps.IsLoggedIn(HttpContext);
            if (!result.Succeeded)
            {
                return _authProps.ChallengeIt();
            }
            var id = _authService.GetSessionId(HttpContext);
            return await _adminsService.GetSpecificAdmin(id);
        }

        [HttpGet("[action]")]
        async public Task<ActionResult> IsIn()
        {
            var result = await _authProps.IsLoggedIn(HttpContext);
            if(result.Succeeded) return _authProps.ReturnUserToMain();
            return Ok();
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> SignUp()
        {
            var result = await _authProps.IsLoggedIn(HttpContext);
            if (!result.Succeeded)
            {
                return _authProps.ChallengeIt();
            }

            List<Admin> admins = await _adminsService.GetAdmins();

            AdminConfirmation adminSignUp = await AdminSignUpRequest();
            Admin newAdmin = new Admin();

           for(int i = 0; i < admins.Count; i++)
            {
                if (String.IsNullOrEmpty(adminSignUp.Email))
                {
                    return "Email cannot be empty.";
                }
                else if (admins[i].Email == adminSignUp.Email)
                {
                    return "Email already in use.";
                }
            }

            if (adminSignUp.Password != adminSignUp.PasswordConfirm)
            {
                return "Passwords do not match.";
            }
            if (adminSignUp.Password.Length < 8 || adminSignUp.PasswordConfirm.Length < 8)
            {
                return "Password should atleast be 8 characters.";
            }

            else
            {
                newAdmin.Email = adminSignUp.Email;
                newAdmin.Password = _authService.AccountProtector("admin-pass", "protect", adminSignUp.Password);

                var newUser = await _adminsService.SignUp(newAdmin);

                if(await _adminsService.GetSpecificAdmin(newUser) != null)
                {
                    await _authService.StoreKey(newUser);
                }
                else
                {
                    Console.WriteLine("Couldn't find the user for key storing!");
                }
            }
            return "Success";
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> SignIn()
        {
            Admin adminRequest = await AdminFromRequest();
            var admin = await _adminsService.GetSpecificAdmin(adminRequest);

            if(admin != null)
            {
                await _authService.RetrieveKey(admin);
                Thread.Sleep(3000);
                string password = _authService.AccountProtector("admin-pass", "unprotect", admin.Password);

                if (password == adminRequest.Password)
                {
                    await _authService.CreateCookie(HttpContext);
                     _authService.SetSession(HttpContext, admin);

                    return "Success";
                }
                else
                {
                    return "Incorrect Password.";
                }
            }
            else
            {
                return "Invalid Email.";
            }     
        }

        [HttpPost("signout")]
        public async new Task SignOut()
        {
            await _authService.DeleteCookies(HttpContext);
        }
    }   
}
