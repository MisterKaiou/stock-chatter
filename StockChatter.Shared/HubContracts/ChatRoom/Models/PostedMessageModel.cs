namespace StockChatter.Shared.HubContracts.ChatRoom.Models
{
	public class PostedMessageModel
	{
		public string Sender { get; set; }
		public string Content { get; set; }
		public DateTime PostedAt { get; set; }
	}
}
