using System.Security.Claims;

namespace StockChatter.Web.Extensions
{
	public static class ClaimsExtensions
	{
		public static string? GetNameFromClaimsPrincipal(this ClaimsPrincipal principal)
		{
			return principal.Claims.FirstOrDefault(c => c.Type.Equals("name", StringComparison.OrdinalIgnoreCase)).Value;
		}
	}
}
