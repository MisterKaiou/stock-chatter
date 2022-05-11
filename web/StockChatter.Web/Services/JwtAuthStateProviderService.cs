using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using StockChatter.Web.Helpers;
using StockChatter.Web.Services.Interfaces;

namespace StockChatter.Web.Services
{
	public class JwtAuthStateProviderService : AuthenticationStateProvider, IJwtStateProviderService
	{
		private const string JWT_AUTH = "JwtBearer";
		private const string JWT_TOKEN = "jwtToken";

		private readonly ILocalStorageService _localStorage;
		private readonly AuthenticationState _anonymous;

		public JwtAuthStateProviderService(ILocalStorageService localStorage)
		{
			_localStorage = localStorage;
			_anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var token = await _localStorage.GetItemAsStringAsync(JWT_TOKEN);

			if (string.IsNullOrWhiteSpace(token))
				return _anonymous;

			return new AuthenticationState(
				new ClaimsPrincipal(
				new ClaimsIdentity(
						JwtParser.ParseClaimFromJwt(token), JWT_AUTH
					)
				)
			);
		}

		public async Task NotifyUserAuthentication(string token)
		{
			await _localStorage.SetItemAsStringAsync(JWT_TOKEN, token);

			var authenticatedUser = new ClaimsPrincipal(
				new ClaimsIdentity(
					JwtParser.ParseClaimFromJwt(token), JWT_AUTH
				)
			);
			var authenticationState = Task.FromResult(new AuthenticationState(authenticatedUser));
			NotifyAuthenticationStateChanged(authenticationState);
		}

		public void NotifyUserLogout()
		{
			_localStorage.RemoveItemAsync(JWT_TOKEN);
			var authenticationState = Task.FromResult(_anonymous);
			NotifyAuthenticationStateChanged(authenticationState);
		}
	}
}
