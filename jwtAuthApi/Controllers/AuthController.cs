using Azure.Core;
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
            response.RefreshToken = token.RefershToken.Token;

            _dataAccess.DisableUserTokenByEmail(request.Email);

            _dataAccess.AddRefreshToken(token.RefershToken, request.Email);

            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public ActionResult<AuthResponse> RefreshToken()
        {
            AuthResponse response = new();

            var refreshToken = Request.Cookies["refreshtoken"];

            if(string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest();
            }

            var isTokenValid = _dataAccess.IsRefreshTokenValid(refreshToken);

            if(!isTokenValid)
            {
                return BadRequest();
            }

            var currentUser = _dataAccess.FindUserByToken(refreshToken);

            if(currentUser == null)
            {
                return BadRequest();
            }

            // Generate access token
            var token = _tokenProvider.GenerateToken(currentUser);

            response.AccessToken = token.AccessToken;

            response.RefreshToken = token.RefershToken.Token;

            _dataAccess.DisableUserToken(refreshToken);

            _dataAccess.AddRefreshToken(token.RefershToken, currentUser.Email);

            return Ok(response);
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            var resfreshToken = Request.Cookies["refreshtoken"];

            if(resfreshToken != null)
            {
                _dataAccess.DisableUserToken(resfreshToken);
            }

            return NoContent();
        }

    }
}
