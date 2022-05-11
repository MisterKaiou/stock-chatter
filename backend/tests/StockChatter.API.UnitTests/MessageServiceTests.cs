using AutoFixture;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Repositories.Interfaces;
using StockChatter.API.Services;
using StockChatter.API.UnitTests.Fixtures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockChatter.API.UnitTests
{
	public class MessageServiceTests
	{
		private readonly MessageFixture _messageFixture = new();
		private readonly UserFixture _userFixture = new();

		private MessagesService _sut;
		private IUoW _uow; // O.O

		[SetUp]
		public void Setup()
		{
			_uow = Substitute.For<IUoW>();
			_sut = new MessagesService(_uow);
		}

		[Test]
		public async Task FetchMessagesStartingFromAsync_ShouldReturnMessages_WhenThereExistsMessagesNewerThenDateRequested()
		{
			// Arrange
			var users = _userFixture.CreateMany<UserDAO>(10).ToList();
			var messages = _messageFixture
				.CreateMany<MessageDAO>(50)
				.Select((m, i) =>
				{
					var usrIdx = i % users.Count;
					users[usrIdx].Id = m.SenderId; //Links user to messages
					return m;
				})
				.ToList();

			_uow.MessagesRepository.Messages.Returns(messages.AsQueryable());
			_uow.UsersRepository.Users.Returns(users.AsQueryable());
			var threeDaysAgo = DateTime.Today.AddDays(-3);

			// Act
			var fetchedMessages = await _sut.FetchMessagesStartingFromAsync(threeDaysAgo);

			// Assert

			// To avoid rewriting the LINQ query executed on the service here, we will assure only for user existing and message date
			// being greater then three days agora. If we did write the query here, then we would be testing the LINQ library too.
			fetchedMessages.Should().Match(ms => ms.All(m => m.SentAt > threeDaysAgo));
			fetchedMessages.Should().Match(ms => ms.All(m => users.Any(u => u.Id == m.SenderIdentifier)));
			fetchedMessages.Should().BeInAscendingOrder((it,ot) => it.SentAt.CompareTo(ot.SentAt));
		}
	}
}