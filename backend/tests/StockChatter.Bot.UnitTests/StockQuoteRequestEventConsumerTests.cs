using AutoFixture;
using CsvHelper;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NSubstitute;
using System.Net.Http;
using RichardSzalay.MockHttp;
using System.IO;
using System.Globalization;
using System;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;
using MassTransit;
using System.Threading.Tasks;
using System.Linq;

namespace StockChatter.Bot.UnitTests
{
	public class StockQuoteRequestEventConsumerTests
	{
		private readonly Fixture _fixture = new Fixture();

		private HttpClient _httpClientSub;
		private MockHttpMessageHandler _mockHttpMessageHandler;
		private IHostApplicationLifetime _appLifetimeSub;
		private ILogger<StockQuoteRequestEventConsumer> _loggerSub;
		private StockQuoteRequestEventConsumer _sut;

		public StockQuoteRequestEventConsumerTests()
		{
			_fixture.Customize<StockDetails>(composer =>
				composer.FromFactory(
					() => new StockDetails(
						_fixture.Create<string>()[0..6],
						_fixture.Create<DateTime>(),
						_fixture.Create<DateTime>().ToString("T"),
						_fixture.Create<decimal>(),
						_fixture.Create<decimal>(),
						_fixture.Create<decimal>(),
						_fixture.Create<decimal>(),
						_fixture.Create<uint>()
					)
				).OmitAutoProperties()
			);
		}

		[SetUp]
		public void Setup()
		{
			_loggerSub = Substitute.For<ILogger<StockQuoteRequestEventConsumer>>();
			_appLifetimeSub = Substitute.For<IHostApplicationLifetime>();
			_mockHttpMessageHandler = new MockHttpMessageHandler();
			_httpClientSub = new HttpClient(_mockHttpMessageHandler);
			_sut = new StockQuoteRequestEventConsumer(_httpClientSub, _appLifetimeSub, _loggerSub);
		}

		[Test]
		public async Task Consume_ValidMessage_ShouldProcessQuoteRequest()
		{
			// Arrange
			var stockDetails = _fixture.Create<StockDetails>();
			var expectedPublish = new StockDetailsMessage(stockDetails.Symbol, stockDetails.Close);
			var textStream = new MemoryStream();
			using var streamWriter = new StreamWriter(textStream);
			using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<StockDetails>();
			csvWriter.NextRecord();
			csvWriter.WriteRecord(stockDetails);
			csvWriter.NextRecord();
			csvWriter.Flush();
			streamWriter.Flush();

			_mockHttpMessageHandler
				.When("https://stooq.com/q/l/*")
				.Respond(
					"text/csv", streamWriter.BaseStream);

			var context = Substitute.For<ConsumeContext<StockQuoteRequestMessage>>();
			context.Message.Returns(
				 new StockQuoteRequestMessage(Guid.NewGuid(), expectedPublish.Symbol)
			);

			// Act
			await _sut.Consume(context);

			// Assert
			await context.Received().Publish(
				Arg.Is<StockDetailsMessage>(m => m == expectedPublish)
			);
		}

		[Test]
		public async Task Consume_InvalidMessage_ShouldFailAndPublishFailureMessage()
		{
			// Arrange
			var requesterId = Guid.NewGuid();
			var expectedFailure = new StockFetchFailedMessage(
				requesterId,
				StockFetchFailureReason.TickerNotFoundOrDataUnavailable);
			var stockDetails = _fixture.Create<StockDetails>();
			var textStream = new MemoryStream();
			using var streamWriter = new StreamWriter(textStream);
			using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<StockDetails>();
			csvWriter.NextRecord();

			// Response returned in case of invalid paramter.
			"AAPL,N/D,N/D,N/D,N/D,N/D,N/D,N/D".Split(',')
				.ToList()
				.ForEach(s => csvWriter.WriteField(s));

			csvWriter.NextRecord();
			csvWriter.Flush();
			streamWriter.Flush();

			_mockHttpMessageHandler
				.When("https://stooq.com/q/l/*")
				.Respond(
					"text/csv", streamWriter.BaseStream);

			var context = Substitute.For<ConsumeContext<StockQuoteRequestMessage>>();
			context.Message.Returns(
				 new StockQuoteRequestMessage(requesterId, stockDetails.Symbol));

			// Act
			await _sut.Consume(context);

			// Assert
			await context.Received().Publish(
				Arg.Is<StockFetchFailedMessage>(f => f == expectedFailure)
			);
		}

		[Test]
		public async Task Consume_TransienteFailure_ShouldPublishFailureMessage()
		{
			// Arrange
			var requesterId = Guid.NewGuid();
			var expectedFailure = new StockFetchFailedMessage(
				requesterId,
				StockFetchFailureReason.TransientError);
			var stockDetails = _fixture.Create<StockDetails>();
			var expectedPublish = new StockDetailsMessage(stockDetails.Symbol, stockDetails.Close);
			var textStream = new MemoryStream();
			using var streamWriter = new StreamWriter(textStream);
			using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
			csvWriter.WriteHeader<StockDetails>();
			csvWriter.NextRecord();
			csvWriter.WriteRecord(stockDetails);
			csvWriter.NextRecord();
			csvWriter.Flush();
			streamWriter.Flush();

			_mockHttpMessageHandler
				.When("https://stooq.com/q/l/*")
				.Respond(
					"text/csv", streamWriter.BaseStream);

			var context = Substitute.For<ConsumeContext<StockQuoteRequestMessage>>();
			context.Message.Returns(
				 new StockQuoteRequestMessage(requesterId, stockDetails.Symbol));
			context
				.When(c => c.Publish(Arg.Any<StockDetailsMessage>()))
				.Do(_ => throw new Exception("Transient failure"));

			// Act
			await _sut.Consume(context);

			// Assert
			await context.Received().Publish(
				Arg.Is<StockFetchFailedMessage>(f => f == expectedFailure)
			);
		}
	}
}