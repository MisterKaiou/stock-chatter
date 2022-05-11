namespace StockChatter.Shared.HubContracts.ChatRoom.Messages
{
	public enum StockFetchFailureReason
	{
		TransientError,
		TickerNotFoundOrDataUnavailable
	}

	public record StockDetailsMessage(string Symbol, decimal Price);
	public record StockFetchFailedMessage(Guid RefererId, StockFetchFailureReason FailureReason);
	public record StockQuoteRequestMessage(Guid RequesterId, string Stock);
}
