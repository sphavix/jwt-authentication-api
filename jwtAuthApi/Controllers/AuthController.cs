using jwtAuthApi.Infrastructure;
using jwtAuthApi.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtAuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataAccess _dataAccess;
        private readonly TokenProvider _tokenProvider;

        public AuthController(DataAccess dataAccess, TokenProvider tokenProvider)
        {
            _dataAccess = dataAccess;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("register")]
        public ActionResult Register(RegisterRequest request)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var result = _dataAccess.RegisterUser(request.Email, hashedPassword, request.Role);

            if(result)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public ActionResult<AuthResponse> Login(AuthRequest request)
        {
            AuthResponse response = new();

            var user = _dataAccess.FindByEmail(request.Email);

            if(user == null)
            {
                return BadRequest("User does not exist!");
            }

            var verifyPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if(!verifyPassword)
            {
                return BadRequest("Incorrect Password, please try again!");
            }

            // generate access token
            var token = _tokenProvider.GenerateToken(user);

            response.AccessToken = token.AccessToken;

            // generate referesh token

            return response;
        }
    }
}
