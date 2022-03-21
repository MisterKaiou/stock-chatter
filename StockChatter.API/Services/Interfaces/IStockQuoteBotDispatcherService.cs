namespace StockChatter.API.Services.Interfaces
{
	public interface IStockQuoteBotDispatcherService
	{
		Task FetchQuoteAsync(string forStock);
	}
}
