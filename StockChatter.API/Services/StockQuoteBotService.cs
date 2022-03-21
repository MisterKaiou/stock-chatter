using RabbitMQ.Client;
using StockChatter.API.Services.Interfaces;
using System.Text;

namespace StockChatter.API.Services
{
	public class StockQuoteBotService : IStockQuoteBotDispatcherService, IDisposable
	{
		private readonly ILogger<StockQuoteBotService> _logger;
		private readonly IConnection _connection;
		private readonly IModel _channel;

		public StockQuoteBotService(ILogger<StockQuoteBotService> logger)
		{
			_logger = logger;
			try
			{
				var factory = new ConnectionFactory { HostName = "localhost", Port = 5672 };
				_connection = factory.CreateConnection();
				_channel = _connection.CreateModel();

				_channel.QueueDeclare(queue: "stocks",
									 durable: false,
									 exclusive: false,
									 autoDelete: false);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create connection to RabbitMQ Broker");
				throw;
			}
		}

		public Task FetchQuoteAsync(string forStock)
		{
			_channel.BasicPublish(
				exchange: "",
				routingKey: "stocks",
				body: Encoding.UTF8.GetBytes(forStock)
			);

			_logger.LogInformation("Successfully published a quote request for stock [{@stock}] to queue", forStock);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_connection?.Dispose();
			_channel?.Dispose();
		}
	}
}
