namespace StockChatter.Shared.HubContracts.ChatRoom.Messages
{
	public record StockDetailsMessage(string Symbol, decimal Price);
	public record StockFetchFailedMessage(Guid RefererId);
	public record StockQuoteRequestMessage(Guid RequesterId, string Stock);
}
