using Microsoft.AspNetCore.Authorization;
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

        public Task SendMessage(PostMessageModel postMessageModel)
        {
            Console.WriteLine(Context.UserIdentifier);

            var message = new Message(Guid.Parse(Context.UserIdentifier), postMessageModel.Sender, postMessageModel.Text, DateTime.Now);

            // TODO: Insert message to the database.

            var postedMessage = new PostedMessageModel
            {
                Content = message.Content,
                PostedAt = message.SentAt,
                Sender = message.SenderName
            };

            return Clients.All.SendAsync(ChatRoomHubMethods.MessageExchange.Receive, postedMessage);
        }
    }
}
