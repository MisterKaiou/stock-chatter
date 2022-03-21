using AutoFixture.NUnit3;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using NUnit.Framework;
using StockChatter.API.Domain.Entitites.Messages;
using StockChatter.API.Hubs;
using StockChatter.API.Services.Interfaces;
using StockChatter.API.UnitTests.Fixtures;
using StockChatter.Shared.HubContracts.ChatRoom;
using StockChatter.Shared.HubContracts.ChatRoom.Messages;
using StockChatter.Shared.HubContracts.ChatRoom.Models;
using System;
using System.Threading.Tasks;

namespace StockChatter.API.UnitTests
{
	internal class ChatRoomHubTests
	{
		private ChatRoomHub _sut;
		private IPublishEndpoint _endpointSub;
		private IMessagesService _messagesServiceSub;

		[SetUp]
		public void Setup()
		{
			_endpointSub = Substitute.For<IPublishEndpoint>();
			_messagesServiceSub = Substitute.For<IMessagesService>();
			_sut = new ChatRoomHub(_endpointSub, _messagesServiceSub);
			_sut.Context = Substitute.ForPartsOf<HubCallerContext>();
			_sut.Clients = Substitute.For<IHubCallerClients>();
		}

		[Test, AutoData]
		public async Task SendMessage_ShouldPublishCommand_WhenStockQuoteCommandIsReceived(string stock, Guid userId)
		{
			// Arrange
			var stockQuoteCommandMessage = new PostMessageModel(
				"Someone",
				$"/stock={stock}");
			_sut.Context.UserIdentifier.Returns(userId.ToString());
			var expectedMessage = new StockQuoteRequestMessage(
				userId,
				stock);

			// Act
			await _sut.SendMessage(stockQuoteCommandMessage);

			// Assert
			await _endpointSub.Received().Publish(expectedMessage);
		}

		[Test, PostMessageModelAutoData]
		public async Task SendMessage_ShouldSendMessageToClients_WhenAValidMessageIsReceived(PostMessageModel model, Guid userId)
		{
			// Arrange
			_sut.Clients.All.Returns(Substitute.For<IClientProxy>());
			_sut.Context.UserIdentifier.Returns(userId.ToString());

			// Act
			await _sut.SendMessage(model);

			// Assert
			var validateSentMessage = (PostedMessageModel? m) =>
			{
				return m == new PostedMessageModel(model.Sender, model.Content, m.PostedAt);
			};

			await _messagesServiceSub
				.Received()
				.PostMessageAsync(
					// Since we can't predict the server time, we assume that everything else is gonna as we expect, except the time.
					Arg.Is<Message>(m => m == new Message(userId, model.Sender, model.Content, m.SentAt)));
			await _sut.Clients.All
				.Received()
				.SendCoreAsync(
					ChatRoomHubMethods.MessageExchange.Receive,
					Arg.Is<object[]?>(arr => validateSentMessage(arr[0] as PostedMessageModel))!);
		}
	}
}
