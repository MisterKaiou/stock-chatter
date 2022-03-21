namespace StockChatter.Shared.HubContracts.ChatRoom.Messages
{
	public class StockDetailsMessage
	{
		public string Symbol { get; set; }
		public decimal Price { get; set; }
	}
}
