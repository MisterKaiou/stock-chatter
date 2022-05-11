using AutoFixture;
using AutoFixture.NUnit3;
using System;

namespace StockChatter.API.UnitTests.Fixtures
{
	[AttributeUsage(AttributeTargets.Method)]
	internal class PostMessageModelAutoDataAttribute : AutoDataAttribute
	{
		public PostMessageModelAutoDataAttribute() : base(CreateFixture) { }

		private static IFixture CreateFixture()
		{
			var fixture = new MessageFixture();
			return fixture;
		}
	}
}
