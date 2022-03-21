using MassTransit;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console(
		outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}",
		theme: AnsiConsoleTheme.Code)
	.CreateLogger();

var logger = Log.ForContext<Program>();

// See https://aka.ms/new-console-template for more information
logger.Information("Consuming Stocks Quote requests");

using var tokenSource = new CancellationTokenSource();
using var httpHandler = new HttpClientHandler();
var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
{
	cfg.Host("localhost", "/");

	cfg.ReceiveEndpoint("stocks", e =>
	{
		e.Consumer(() =>
			new StockQuoteRequestEventConsumer(
				new HttpClient(httpHandler, false),
				tokenSource.Token,
				Log.ForContext<StockQuoteRequestEventConsumer>()
			)
		);
	});
});

await busControl.StartAsync();

try
{
	logger.Information("Press any key to exit");
	await Task.Run(() => Console.ReadLine());
}
finally
{
	await busControl.StopAsync();
	logger.Information("Application shutting down");
}
