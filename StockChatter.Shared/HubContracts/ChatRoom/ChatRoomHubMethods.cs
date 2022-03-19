namespace StockChatter.Shared.HubContracts.ChatRoom
{
	public static class ChatRoomHubMethods
	{
		public static class MessageExchange
		{
			public const string Receive = "ReceiveMessage";
			public const string Send = "SendMessage";
		}
	}
}