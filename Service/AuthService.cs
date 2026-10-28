using Microsoft.IdentityModel.Tokens;
using projfour.Models;
using projfour.Response_Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace projfour.Service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly List<User> _users =
              [
                  new User
                    {
                        Username = "admin",
                        Password = "admin123",
                        Role = "Admin"
                    },

                    new User
                    {
                        Username = "employee",
                        Password = "employee123",
                        Role = "Employee"
                    }
              ];
        private readonly Dictionary<string, string> _refreshTokens = new();

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public LoginResponse? Login(LoginRequest request)
        {
            var user = _users.FirstOrDefault(x =>
               x.Username == request.Username &&
               x.Password == request.Password);

            if (user == null)
            {
                return null;
            }

            var accessToken = GenerateAccessToken(user);

            var refreshToken = GenerateRefreshToken();

            _refreshTokens[refreshToken] = user.Username;

            return new LoginResponse
            {
                AccessToken = accessToken.Token,
                RefreshToken = refreshToken,
                AccessTokenExpiry = accessToken.Expiry
            };


        }


        public LoginResponse? RefreshToken(string refreshToken)
        {
            if (!_refreshTokens.TryGetValue(refreshToken, out var username))
            {
                return null;
            }

            var user = _users.FirstOrDefault(x =>
                x.Username == username);

            if (user == null)
            {
                return null;
            }

            // Remove old refresh token
            _refreshTokens.Remove(refreshToken);

            var accessToken = GenerateAccessToken(user);

            var newRefreshToken = GenerateRefreshToken();

            _refreshTokens[newRefreshToken] = user.Username;
            
            return new LoginResponse
            {
                AccessToken = accessToken.Token,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = accessToken.Expiry
            };

        }

        private (string Token, DateTime Expiry) GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings["Key"]!;
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var minutes = int.Parse(jwtSettings["AccessTokenMinutes"] ?? "15");
            var expiry = DateTime.UtcNow.AddMinutes(minutes);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role),

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Username)
            };


            var securityKey =
               new SymmetricSecurityKey(
                   Encoding.UTF8.GetBytes(key));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            return (
                new JwtSecurityTokenHandler().WriteToken(token),
                expiry
            );

        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var random = RandomNumberGenerator.Create();
            random.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);

        }







        private class User
        {
            public string Username { get; set; } = string.Empty;

            public string Password { get; set; } = string.Empty;

            public string Role { get; set; } = string.Empty;
        }






    }
}
