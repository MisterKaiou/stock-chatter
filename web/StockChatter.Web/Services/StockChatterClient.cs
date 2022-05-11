using Blazored.LocalStorage;
using System.Net.Mime;
using System.Net.Http.Headers;

namespace StockChatter.Web.Services
{
    public class StockChatterClient
    {
		private readonly ILocalStorageService _localStorage;

		protected HttpClient HttpClient { get; }

        public StockChatterClient(HttpClient httpClient, ILocalStorageService localStorage)
		{
			HttpClient = httpClient;
			HttpClient.DefaultRequestHeaders.Accept.Add(
				 new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
			_localStorage = localStorage;
		}

		public async Task<TResponse> SendRequestAsync<TResponse>(Func<HttpClient, Task<TResponse>> request)
		{
			await LoadCredentials();
			return await request(HttpClient);
		}

		protected virtual async Task LoadCredentials()
		{
			var token = await _localStorage.GetItemAsStringAsync("jwtToken") ?? string.Empty;
			HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
		}
	}
}
