﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockChatter.API.Domain.Entitites.Messages;
using StockChatter.API.Infrastructure.Services.Interfaces;
using StockChatter.Shared.HubContracts.ChatRoom;

namespace StockChatter.API.Hubs
{
	[Authorize]
	public class ChatRoomHub : Hub
	{
		private readonly IMessagesService _messagesService;

		public ChatRoomHub(IMessagesService messagesService)
		{
			_messagesService = messagesService;
		}

		public async Task SendMessage(PostMessageModel postMessageModel)
		{
			var message = new Message(Guid.Parse(Context.UserIdentifier), postMessageModel.Sender, postMessageModel.Text, DateTime.Now);

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
