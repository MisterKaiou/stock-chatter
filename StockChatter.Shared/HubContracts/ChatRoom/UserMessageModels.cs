namespace StockChatter.Shared.HubContracts.ChatRoom.Models
{
	public record PostedMessageModel(string Sender, string Content, DateTime PostedAt);

	public record PostMessageModel(string Sender, string Content);
}
