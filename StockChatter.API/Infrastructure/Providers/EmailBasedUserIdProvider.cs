using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace StockChatter.API.Infrastructure.Providers
{
	public class EmailBasedUserIdProvider : IUserIdProvider
	{
		public string? GetUserId(HubConnectionContext connection)
		{
			return connection
				.User
				.Claims
				.First(c => c.Properties.Values.Contains(JwtRegisteredClaimNames.NameId)).Value;
		}
	}
}
