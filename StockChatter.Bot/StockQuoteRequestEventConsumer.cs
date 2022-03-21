using CsvHelper;
using MassTransit;
using System.Globalization;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public record StockDetails(string Symbol, DateTime Date, string Time, decimal Open, decimal High, decimal Low, decimal Close, uint Volume);
public class StockQuoteRequestEventConsumer : IConsumer<StockQuoteRequestMessage>, IDisposable
{
	private readonly CancellationTokenSource _tokenSource = new();

	private readonly HttpClient _httpClient;
	private readonly ILogger<StockQuoteRequestEventConsumer> _logger;

	public StockQuoteRequestEventConsumer(HttpClient httpClient, IHostApplicationLifetime applicationLifetime, ILogger<StockQuoteRequestEventConsumer> logger)
	{
		_httpClient = httpClient;
		_logger = logger;
		applicationLifetime.ApplicationStopping.Register(() => _tokenSource.Cancel());
		_logger = logger;
		_httpClient.BaseAddress = new Uri("https://stooq.com");
	}

	public async Task Consume(ConsumeContext<StockQuoteRequestMessage> context)
	{
		var requestedStock = context.Message.Stock;
		_logger.LogInformation("Quote request received for stock: [{@requestedStock}]", requestedStock);

		try
		{
			var response = await _httpClient.GetStreamAsync($"q/l/?s={requestedStock}&f=sd2t2ohlcv&h&e=csv", _tokenSource.Token);
			using var textReader = new StreamReader(response);
			using var csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture);

			var stockDetails = csvReader.GetRecords<StockDetails>().First();

			await context.Publish(
				   new StockDetailsMessage(stockDetails.Symbol, stockDetails.Close)
			);

			_logger.LogInformation("Successfully processed quote request for stock: [{@requestedStock}]", requestedStock);
		}
		catch (ReaderException csvEx)
		{
			_logger.LogError(
				csvEx,
				  "Failed to parse response, stock [{@requestedStock}] unknown or no data available",
				  requestedStock
			);

			await context.Publish(
				   new StockFetchFailedMessage(context.Message.RequesterId)
			);
		}
		catch (Exception ex)
		{
			_logger.LogError(
				ex,
				 "Something went wrong when attempting to fetch stock quote for stock [{@requestedStock}]",
				 requestedStock
			);
		}
	}

	public void Dispose() => _httpClient.Dispose();
}
