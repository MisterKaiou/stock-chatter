using Functional.Result;
using System.Net.Http.Json;
using StockChatter.Services.Interfaces;
using StockChatter.Shared.Models.Auth;
using StockChatter.Extensions;

namespace StockChatter.Services
{
	public class AuthService : IAuthService
	{
		private readonly StockChatterClient _client;

		public AuthService(StockChatterClient client) 
		{
			_client = client;
		}

		public async Task<Result<Unit>> RegisterUserAsync(RegistrationRequest request)
		{
			var response = await _client.SendRequestAsync(c => c.PostAsJsonAsync("auth/register", request));
			return await response.HttpResponseToResultAsync();
		}

		public async Task<Result<string>> LoginUserAsync(LoginRequest request)
		{
			var response = await _client.SendRequestAsync(c => c.PostAsJsonAsync("auth/login", request));
			return await response.HttpResponseToResultAsync(
				c => c.ReadAsStringAsync()
			);
		}
	}
}
