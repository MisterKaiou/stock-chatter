using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockChatter.API.Domain.Entitites.Messages;
using StockChatter.API.Services.Interfaces;
using StockChatter.Shared.HubContracts.ChatRoom;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;
using StockChatter.Shared.HubContracts.ChatRoom.Models;
using System.Text.RegularExpressions;

namespace StockChatter.API.Hubs
{
	[Authorize]
	public class ChatRoomHub : Hub
	{
		private readonly static Regex _stockCommandRgx = new Regex(@"^\/stock=(?'stock'\S+)$", RegexOptions.Compiled);

		private readonly IPublishEndpoint _publisher;
		private readonly IMessagesService _messagesService;

		public ChatRoomHub(IPublishEndpoint botDispatcher, IMessagesService messagesService)
		{
			_publisher = botDispatcher;
			_messagesService = messagesService;
		}

		public async Task SendMessage(PostMessageModel postMessageModel)
		{
			var userId = Guid.Parse(Context.UserIdentifier);

			var commandMatch = _stockCommandRgx.Match(postMessageModel.Content);
			if (commandMatch.Success)
			{
				await _publisher.Publish(new StockQuoteRequestMessage
				{
					Stock = commandMatch.Groups["stock"].Value,
					RequesterId = userId
				});
				return;
			}

			var message = new Message(userId, postMessageModel.Sender, postMessageModel.Content, DateTime.Now);

			await _messagesService.PostMessageAsync(message);

			var postedMessage = new PostedMessageModel
			{
				Content = message.Content,
				PostedAt = message.SentAt,
				Sender = message.SenderName
			};

			await Clients.All.SendAsync(ChatRoomHubMethods.MessageExchange.Receive, postedMessage);
		}

		public async Task SyncMessages(DateTime startingFrom)
		{
			var messages = await _messagesService.FetchMessagesStartingFrom(startingFrom);

			await Clients.Caller.SendAsync(
				ChatRoomHubMethods.MessageExchange.SyncClient,
				messages.Select(m => new PostedMessageModel
				{
					Content = m.Content,
					PostedAt = m.SentAt,
					Sender = m.SenderName
				})
			);
		}
	}
}
