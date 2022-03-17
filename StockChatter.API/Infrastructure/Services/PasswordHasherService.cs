using Microsoft.AspNetCore.Identity;
using Sodium;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Services
{
	public class PasswordHasherService : IPasswordHasher<UserDAO>
	{
		public string HashPassword(UserDAO user, string password)
		{

			return PasswordHash.ArgonHashString(password,
#if DEBUG
				PasswordHash.StrengthArgon.Interactive
#else
                PasswordHash.StrengthArgon.Medium
#endif
				);
		}

		public PasswordVerificationResult VerifyHashedPassword(UserDAO user, string hashedPassword, string providedPassword)
		{
			if (PasswordHash.ArgonPasswordNeedsRehash(hashedPassword,
#if DEBUG
				PasswordHash.StrengthArgon.Interactive
#else
                PasswordHash.StrengthArgon.Medium
#endif
))
				return PasswordVerificationResult.SuccessRehashNeeded;

			if (PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword))
				return PasswordVerificationResult.Success;

			return PasswordVerificationResult.Failed;
		}
	}
}
