using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using StockChatter.Shared.HubContracts.ChatRoom;

namespace StockChatter.HubClients
{
	public delegate void MessageReceived(PostedMessageModel chatMessage);

	public class ChatRoomHubClient : IAsyncDisposable
	{
		private HubConnection? _hub;
		private readonly ILocalStorageService _localStorage;
		private readonly string _baseUri;

		public event MessageReceived? OnMessageReceived;

		public HubConnectionState ConnectionState => _hub.State;

		public ChatRoomHubClient(IConfiguration configuration, ILocalStorageService localStorage)
		{
			_localStorage = localStorage;
			_baseUri = configuration.GetValue<string>("ApiBaseUri");
		}

		public async Task SendMessage(PostMessageModel message)
		{
			await _hub.SendAsync(ChatRoomHubMethods.MessageExchange.Send, message);
		}

		public async Task StartAsync()
		{
			_hub = new HubConnectionBuilder()
				.WithUrl($"{_baseUri}/chatRoom", async opt =>
				{
					opt.AccessTokenProvider =
						async () => await _localStorage.GetItemAsStringAsync("jwtToken"); //TODO: Change to constant
				})
				.WithAutomaticReconnect(new[] { TimeSpan.FromSeconds(2) })
				.Build();

			SetupEvents();
			await _hub.StartAsync();
		}

		public async ValueTask DisposeAsync()
		{
			if (_hub is not null)
				await _hub.DisposeAsync();
		}

		private void SetupEvents()
		{
			_hub.On<PostedMessageModel>(ChatRoomHubMethods.MessageExchange.Receive,
				message => OnMessageReceived?.Invoke(message)
			);
		}
	}
}
