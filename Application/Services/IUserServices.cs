using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoWeb.Application.Dtos.UserDTO;
using TodoWeb.Application.Helpers;
using TodoWeb.Domain.AppsetingsConfigurations;
using TodoWeb.Domain.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services
{
    public interface IUserServices
    {
        public IActionResult Register(UserCreateModel user);
        public User? Login(UserLoginModel user);

        public User GetUserByEmail(string email);

        string GenerateJwt(User user);

        string GenerateRefreshToken(int userId);

        User GetUserByRefreshToken(string refreshToken);
        void DeleteOldRefreshTokens(int userId);
    }

    public class UserServices : IUserServices
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        public UserServices(IApplicationDbContext dbContext, IOptions<JwtSettings> jwtSettingOptions)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettingOptions.Value;
        }

        public string GenerateJwt(User user)
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new (ClaimTypes.Email, user.EmailAddress),
                new (ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256Signature)
            );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        // Salting: nó chỉ là một chuỗi kí tự
        // lấy password ban đầu cộng với cái chuỗi salting này
        public User? Login(UserLoginModel loginModel)
        {
            var user = _dbContext.Users
                .FirstOrDefault(x => x.EmailAddress == loginModel.EmailAddress);
            if (user == null)
            {
                return null;
            }
            var password = loginModel.Password + user.Salting;

            if (!HashHelper.BCryptVerify(password, user.Password))
            {
                return null;
            }
            return user;
        }

        public IActionResult Register(UserCreateModel user)
        {
            if (user == null)
            {
                return new BadRequestObjectResult("User is Empty");
            }
            if (string.IsNullOrEmpty(user.EmailAddress) || string.IsNullOrEmpty(user.Password))
            {
                return new BadRequestObjectResult("Email or Password is Empty");
            }

            if (_dbContext.Users.Any(x => x.EmailAddress == user.EmailAddress))
            {
                return new BadRequestObjectResult("Email is already exist");
            }

            var salting = HashHelper.GenerateRandomString(100);
            var password = HashHelper.HashBcrypt(user.Password + salting);
            var newUser = new User
            {
                EmailAddress = user.EmailAddress,
                Password = password,
                FullName = user.FullName,
                Role = (Domain.Entities.Role)user.Role,
                Salting = salting
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return new OkObjectResult("User created successfully");
        }

        public User GetUserByEmail(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.EmailAddress == email);
        }

        //cái này đơn giản là một string thôi
        public string GenerateRefreshToken(int userId)
        {
            string refreshToken = HashHelper.GenerateRandomString(64);

            string hashedRefreshToken = HashHelper.HashSha256(refreshToken);

            var data = new RefreshToken
            {
                UserId = userId,
                Token = hashedRefreshToken,
                Expiration = DateTime.UtcNow.AddDays(7), // Token có hiệu lực trong 7 ngày
                IsRevoked = false // Mặc định là chưa bị thu hồi
            };
            _dbContext.RefreshTokens.Add(data);
            _dbContext.SaveChanges();
            
            return hashedRefreshToken; // tra về token đã được băm vì sao vậy
        }

        //bug
        public void DeleteOldRefreshTokens(int userId)
        {
            var refreshTokens = _dbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToList();
            if (refreshTokens.Count == 0)
            {
                return; // Không có token nào để xóa
            }
            // Xoas het refresh token của người dùng
            _dbContext.RefreshTokens.RemoveRange(refreshTokens);
            _dbContext.SaveChanges();

        }

        public User GetUserByRefreshToken(string refreshToken)
        {
            var user = _dbContext.RefreshTokens
                .Where(rt => rt.Token == HashHelper.HashSha256(refreshToken) && !rt.IsRevoked && rt.Expiration > DateTime.Now)
                .Select(rt => rt.User)
                .FirstOrDefault();


            return user ?? throw new Exception("Refresh token is invalid or has been revoked.");
        }
    }
}
