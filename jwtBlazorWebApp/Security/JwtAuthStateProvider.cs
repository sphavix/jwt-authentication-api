using jwtBlazorWebApp.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace jwtBlazorWebApp.Security
{
    public class JwtAuthStateProvider(
        AccessTokenService accessTokenService) : AuthenticationStateProvider
    {
        private readonly AccessTokenService _accessTokenService = accessTokenService;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _accessTokenService.GetToken();
                if(string.IsNullOrWhiteSpace(token))
                {
                    await MarkAsUnauthorized();
                }

                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);

                var identity = new ClaimsIdentity(jwtToken.Claims, "JWT");

                var principal = new ClaimsPrincipal(identity);

                return await Task.FromResult(new AuthenticationState(principal));

            }
            catch(Exception ex)
            {
                return await MarkAsUnauthorized();
            }
        }

        private async Task<AuthenticationState> MarkAsUnauthorized()
        {
            try
            {
                var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

                NotifyAuthenticationStateChanged(Task.FromResult(state));

                return state;
            }
            catch(Exception ex)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
    }
}
