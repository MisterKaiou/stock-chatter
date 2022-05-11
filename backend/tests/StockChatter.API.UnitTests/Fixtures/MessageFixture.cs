using AutoFixture;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.Shared.HubContracts.ChatRoom.Models;
using System;

namespace StockChatter.API.UnitTests.Fixtures
{
	/// <summary>
	/// Fixture for anything message, DAO's, Domains, events, etc
	/// </summary>
	internal class MessageFixture : Fixture
	{
		public MessageFixture()
		{
			Customizations.Add(new RandomDateTimeSequenceGenerator(
				DateTime.Now.AddDays(-7),
				DateTime.Now));

			Customize<PostMessageModel>(c =>
				c.WithAutoProperties());

			Customize<MessageDAO>(c =>
				c.WithAutoProperties());
		}
	}
}
