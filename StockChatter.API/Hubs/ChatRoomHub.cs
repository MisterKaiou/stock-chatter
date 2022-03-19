using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StockChatter.Shared.HubContracts.ChatRoom;

namespace StockChatter.API.Hubs
{
	[Authorize]
	public class ChatRoomHub : Hub
	{
		public Task SendMessage(ChatMessage message) 
			=> Clients.All.SendAsync(ChatRoomHubMethods.MessageExchange.Receive, message);
	}
}
