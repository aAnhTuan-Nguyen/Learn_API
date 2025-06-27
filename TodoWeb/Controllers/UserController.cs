using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoWeb.Application.Dtos;
using TodoWeb.Application.Dtos.UserDTO;
using TodoWeb.Application.Services;
using TodoWeb.Application.Services.MiddlwareServices;
using TodoWeb.Infrastructures;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private readonly IGoogleCredentialService _googleCredentialService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        public UserController(IUserServices userServices, IGoogleCredentialService googleCredentialService)
        {
            _userServices = userServices;
            _googleCredentialService = googleCredentialService;
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

            // delete old refresh token
            _userServices.DeleteOldRefreshTokens(user.Id);
            // Tạo JWT token cho người dùng

            var accessToken = _userServices.GenerateJwt(user);
            var refreshToken = _userServices.GenerateRefreshToken(user.Id);
            // set refresh token to cookie or session
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Chỉ gửi cookie qua HTTPS
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Thời gian hết hạn của refresh token
            };
            HttpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            return Ok(accessToken);
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            var isExist = HttpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken);
            if (!isExist)
            {
                return BadRequest("Refresh token not found");
            }
            var user = _userServices.GetUserByRefreshToken(refreshToken!);
            if (user == null)
            {
                return BadRequest("Refresh token is invalid or has been revoked.");
            }
            // delete old refresh token
            _userServices.DeleteOldRefreshTokens(user.Id);

            // Tạo refresh token mới
            var newRefreshToken = _userServices.GenerateRefreshToken(user.Id);
            if (newRefreshToken == null)
            {
                return BadRequest("Failed to generate new refresh token.");
            }
            // Lưu refresh token mới vào cơ sở dữ liệu (nếu cần thiết)
            

            // Cập nhật refresh token mới vào cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Chỉ gửi cookie qua HTTPS
                Expires = DateTimeOffset.UtcNow.AddDays(7) // Thời gian hết hạn của refresh token
            };
            HttpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);

            // Tạo access token mới
            var newAccessToken = _userServices.GenerateJwt(user);

            return Ok(newAccessToken);
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
            var clientId = "137320849289-d8q97maftuq347tslj5276bjl1lc3jp5.apps.googleusercontent.com"; // Lấy từ cấu hình ứng dụng
            var payload = await _googleCredentialService.VerifyCredential(clientId, model.Credential);

            if (payload == null)
            {
                return BadRequest("Invalid Google credential");
            }

            var user = _userServices.GetUserByEmail(payload.Email);
            if (user == null)
            {
                var newUser = new UserCreateModel
                {
                    EmailAddress = payload.Email,
                    FullName = payload.Name,
                    Role = Role.Stud, // Vai trò mặc định
                };

                var result = _userServices.Register(newUser);
                if (result is BadRequestObjectResult badRequest)
                {
                    return BadRequest(badRequest.Value);
                }

                user = _userServices.GetUserByEmail(newUser.EmailAddress);
            }

            // Tạo JWT token cho người dùng
            var token = _userServices.GenerateJwt(user);
            return Ok(new { Token = token, User = user });
        }

        // Cache build in .Net, Time to live
        // Set user to InActive
        // Delete Refresh Token
        // User still keep access token, assume expire time is 15 minutes

        // BAN user's access token
        // Create a authorize filter, cache contains black list access token
        // Filter check if user's access token exist in the cache
        // Yes => return Unauthorize
        // No => continue processing
        [HttpPost("Logout-jwt")]
        public IActionResult LogoutJwt()
        {
            // Xoá refresh token khỏi cookie
            HttpContext.Response.Cookies.Delete("RefreshToken");
            return Ok("Logout successfully");
        }

        [HttpPost("Revoke")]
        public IActionResult BanUserToken([FromBody] string accessToken)
        {

            _tokenBlacklistService.BanToken(accessToken);
            return Ok("Token has been banned.");
        }

    }
}
