using AutoFixture;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.UnitTests.Fixtures
{
	/// <summary>
	/// Fixture for anything User, DAO's, Domains, events, etc
	/// </summary>
	internal class UserFixture : Fixture
	{
		public UserFixture()
		{
			Customize<UserDAO>(
				c => c.WithAutoProperties());
		}
	}
}
