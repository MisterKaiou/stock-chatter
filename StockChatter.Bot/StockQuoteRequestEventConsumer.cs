using CsvHelper;
using MassTransit;
using StockChatter.Shared.HubContracts.ChatRoom;
using Serilog;
using System.Globalization;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;

public class StockQuoteRequestEventConsumer : IConsumer<StockQuoteRequestMessage>, IDisposable
{
	private readonly HttpClient _httpClient;
	private readonly CancellationToken _token;
	private readonly ILogger _logger;

	public StockQuoteRequestEventConsumer(HttpClient httpClient, CancellationToken token, ILogger logger)
	{
		_httpClient = httpClient;
		_token = token;
		_logger = logger;
		_httpClient.BaseAddress = new Uri("https://stooq.com");
	}

	public async Task Consume(ConsumeContext<StockQuoteRequestMessage> context)
	{
		var requestedStock = context.Message.Stock;
		_logger.Information("Quote request received for stock: [{@requestedStock}]", requestedStock);

		try
		{
			var response = await _httpClient.GetStreamAsync($"q/l/?s={requestedStock}&f=sd2t2ohlcv&h&e=csv", _token);
			using var textReader = new StreamReader(response);
			using var csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture);

			var stockDetails = csvReader.GetRecords<StockDetails>().First();

			await context.Publish(new StockDetailsMessage
			{
				Symbol = stockDetails.Symbol,
				Price = stockDetails.Close
			});

			_logger.Information("Successfully processed quote request for stock: [{@requestedStock}]", requestedStock);
		}
		catch (ReaderException csvEx)
		{
			_logger.Error(
				csvEx,
				  "Failed to parse response, stock [{@requestedStock}] unknown or no data available",
				  requestedStock
			);

			await context.Publish(new StockFetchFailedMessage
			{
				RefererId = context.Message.RequesterId
			});
		}
		catch (Exception ex)
		{
			_logger.Error(
				ex,
				 "Something went wrong when attempting to fetch stock quote for stock [{@requestedStock}]",
				 requestedStock
			);
		}
	}

	public void Dispose() => _httpClient.Dispose();

	private record StockDetails(string Symbol, DateTime Date, string Time, decimal Open, decimal High, decimal Low, decimal Close, uint Volume);
}
