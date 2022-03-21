namespace StockChatter.Shared.HubContracts.ChatRoom.Messages
{
	public class StockQuoteRequestMessage
	{
		public Guid RequesterId { get; set; }
		public string Stock { get; set; }
	}
}
