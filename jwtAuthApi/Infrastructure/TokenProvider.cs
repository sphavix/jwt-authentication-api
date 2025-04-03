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

        public Token GenerateToken(User user)
        {
            var accessToken = GenereteAccessToken(user);

            return new Token { AccessToken = accessToken };
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
    }
}
