using jwtAuthApi.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace jwtAuthApi.Infrastructure
{
    public class TokenProvider(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public UserToken GenerateToken(User user)
        {
            var accessToken = GenereteAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            return new UserToken 
            { 
                AccessToken = accessToken,
                RefershToken = refreshToken
            };
        }

        private string GenereteAccessToken(User user)
        {
            var secretKey = _configuration["JWT:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
                ]),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now.AddMonths(1),
                CreatedAt = DateTime.Now,
                Enabled = true
            };

            return refreshToken;
        }
    }
}
