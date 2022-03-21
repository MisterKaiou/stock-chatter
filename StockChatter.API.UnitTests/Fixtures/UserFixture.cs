using AutoFixture;
using StockChatter.API.Infrastructure.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
