using MassTransit;
using Microsoft.AspNetCore.SignalR;
using StockChatter.API.Hubs;
using StockChatter.Shared.HubContracts.ChatRoom;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;
using StockChatter.Shared.HubContracts.ChatRoom.Models;

namespace StockChatter.API.Services
{
	public class StocksConsumerService : IConsumer<StockDetailsMessage>, IConsumer<StockFetchFailedMessage>
	{
		private const string BOT_NAME = "Market Bot (Bob)";

		private readonly IHubContext<ChatRoomHub> _hubContext;
		private readonly ILogger<StocksConsumerService> _logger;

		public StocksConsumerService(IHubContext<ChatRoomHub> hubContext, ILogger<StocksConsumerService> logger)
		{
			_hubContext = hubContext;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<StockDetailsMessage> context)
		{
			var stock = context.Message;
			_logger.LogInformation("Notifying users of quote for stock [{@symbol}]", stock.Symbol);

			await _hubContext.Clients.All.SendAsync(
				ChatRoomHubMethods.MessageExchange.Receive, new PostedMessageModel
				{
					Content = $"{stock.Symbol.ToUpperInvariant()} quote is ${stock.Price} per share",
					PostedAt = DateTime.Now,
					Sender = BOT_NAME
				}
			);
		}

		public async Task Consume(ConsumeContext<StockFetchFailedMessage> context)
		{
			await _hubContext.Clients
				.User(context.Message.RefererId.ToString())
				.SendAsync(ChatRoomHubMethods.MessageExchange.Receive, new PostedMessageModel
				{
					Sender = BOT_NAME,
					Content = "Sorry, I didn't undestand your ticker",
					PostedAt = DateTime.Now
				});
		}
	}
}
