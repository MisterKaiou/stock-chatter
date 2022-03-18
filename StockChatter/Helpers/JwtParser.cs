using System.Security.Claims;
using System.Text.Json;

namespace StockChatter.Helpers
{
	public static class JwtParser
	{
		public static IEnumerable<Claim> ParseClaimFromJwt(string jwt)
        {
			var payload = jwt.Split('.')[1];

			var bytes = ParseBase64WithoutPadding(payload);
			return 
				JsonSerializer
					.Deserialize<Dictionary<string, object>>(bytes)
					?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()))
					?? Array.Empty<Claim>();
        }

		private static byte[] ParseBase64WithoutPadding(string base64)
        {
            static byte[] ToBytes(string base64String) => Convert.FromBase64String(base64String);

			return (base64.Length % 4) switch
			{
				2 => ToBytes(base64 += "=="),
				3 => ToBytes(base64 += "="),
				_ => ToBytes(base64),
			};
        }
	}
}
