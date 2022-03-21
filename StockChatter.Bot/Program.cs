using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console(
		outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}",
		theme: AnsiConsoleTheme.Code)
	.CreateBootstrapLogger();

var logger = Log.ForContext<Program>();

try
{
	await Host.CreateDefaultBuilder(args)
	.UseSerilog((ctx, cfg) =>
	{
		cfg.MinimumLevel.Debug()
			.WriteTo.Console(
				outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} <s:{SourceContext}>{NewLine}{Exception}",
				theme: AnsiConsoleTheme.Code);
	})
	.ConfigureServices(services =>
	{
		services.AddSingleton<HttpClientHandler>();
		services.AddTransient(svcs => new HttpClient(svcs.GetRequiredService<HttpClientHandler>(), false));

		services.AddMassTransit(massCfg =>
		{
			massCfg.AddConsumer<StockQuoteRequestEventConsumer>();

			massCfg.UsingRabbitMq((busCtx, rabbitCfg) =>
			{
				rabbitCfg.Host(Environment.GetEnvironmentVariable("QUEUE_HOST_NAME") ?? "localhost", "/");

				rabbitCfg.ReceiveEndpoint("stocks", e =>
				{
					e.ConfigureConsumer<StockQuoteRequestEventConsumer>(busCtx);
				});
			});
		});
	})
	.RunConsoleAsync();
}
catch (Exception ex)
{
	logger.Error(ex, "Something went wrong when starting the application");
}

logger.Information("Application shutting down");
