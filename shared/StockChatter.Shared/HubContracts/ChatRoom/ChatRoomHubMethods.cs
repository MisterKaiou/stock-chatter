namespace StockChatter.Shared.HubContracts.ChatRoom
{
	public static class ChatRoomHubMethods
	{
		public static class MessageExchange
		{
			public const string Receive = "ReceiveMessage";
			public const string Send = "SendMessage";
			public const string Sync = "SyncMessages";
			public const string SyncClient = "SyncClient";
		}
	}
}