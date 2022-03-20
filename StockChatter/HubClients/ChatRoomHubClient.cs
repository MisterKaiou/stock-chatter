using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using StockChatter.Shared.HubContracts.ChatRoom;

namespace StockChatter.HubClients
{
	public class ChatRoomHubClient : IAsyncDisposable
	{
		public delegate void MessageReceived(PostedMessageModel chatMessage);
		public delegate void MessagesSynchronized(IEnumerable<PostedMessageModel> chatMessage);

		private HubConnection? _hub;
		private readonly ILocalStorageService _localStorage;
		private readonly string _baseUri;

		public event MessageReceived? OnMessageReceived;
		public event MessagesSynchronized? OnMessagesSynchronized;

		public HubConnectionState ConnectionState => _hub.State;

		public ChatRoomHubClient(IConfiguration configuration, ILocalStorageService localStorage)
		{
			_localStorage = localStorage;
			_baseUri = configuration.GetValue<string>("ApiBaseUri");
		}

		public async Task SendMessageAsync(PostMessageModel message)
		{
			await _hub.SendAsync(ChatRoomHubMethods.MessageExchange.Send, message);
		}

		public async Task SyncMessagesStartingAtAsync(DateTime date)
		{
			await _hub.SendAsync(ChatRoomHubMethods.MessageExchange.Sync, date);
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

		public async Task StopAsync()
		{
			if (_hub == null) return;

			await _hub.StopAsync();
			_hub?.Remove(ChatRoomHubMethods.MessageExchange.Receive);
			_hub?.Remove(ChatRoomHubMethods.MessageExchange.Sync);
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

			_hub.On<IEnumerable<PostedMessageModel>>(ChatRoomHubMethods.MessageExchange.SyncClient,
				messages => OnMessagesSynchronized?.Invoke(messages)
			);
		}
	}
}
