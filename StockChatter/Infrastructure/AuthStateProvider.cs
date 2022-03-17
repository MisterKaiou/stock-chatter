using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace StockChatter.Infrastructure
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        public AuthStateProvider()
        {

        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var anonymous = new ClaimsIdentity();
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(anonymous)));
        }
    }
}
