using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Dtos.UserDTO;
using TodoWeb.Application.Services;
using TodoWeb.Infrastructures;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        //private readonly IApplicationDbContext _dbContext;
        private readonly IGoogleCredentialService _googleCredentialService;
        public UserController(IUserServices userServices, IGoogleCredentialService googleCredentialService)
        {
            _userServices = userServices;
            _googleCredentialService = googleCredentialService;
            //_dbContext = dbContext;
        }

        [HttpPost("Register")]
        public IActionResult Register(UserCreateModel user)
        {
            // hash password then compare in database
            return _userServices.Register(user);
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginModel loginModel)
        {
            var user = _userServices.Login(loginModel);
            if (user == null)
            {
                return BadRequest("Password or Email is in correct");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());

            return Ok("Login successfully");
        }

        [HttpPost("Login-cookies")]
        public async Task<IActionResult> LoginCookies(UserLoginModel loginModel)
        {
            var user = _userServices.Login(loginModel);
            if (user == null)
            {
                return BadRequest("Password or Email is in correct");
            }
            
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.EmailAddress),
                new (ClaimTypes.Role, user.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok("Login successfully");
        }
        [HttpPost("Login-jwt")]
        public async Task<IActionResult> LoginJwt(UserLoginModel loginModel)
        {
            var user = _userServices.Login(loginModel);
            if (user == null)
            {
                return BadRequest("Password or Email is in correct");
            }

            var token = _userServices.GenerateJwt(user);

            return Ok(token);
        }


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok("Logout successfully");
        }

        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("Role");
            if (userId == null || role == null)
            {
                return BadRequest("User is not logged in");
            }
            return Ok(new { UserId = userId, Role = role });
        }

        [HttpPost("Login-Google")]
        public async Task<IActionResult> LoginGoogle(GoogleLoginModel model)
        {
            // Todo: get from appsettings
            var clientId = "137320849289-d8q97maftuq347tslj5276bjl1lc3jp5.apps.googleusercontent.com"; // Replace with your Google Client ID
            var payload = await _googleCredentialService.VerifyCredential(clientId, model.Credential);

            // Register user if not exists
            
            // Generate JWT token for the user

            //var jwt = _userServices.GenerateJwt(user)

            return Ok(payload);
        }
    }
}
