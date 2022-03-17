using Functional.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StockChatter.API.Domain.Entitites;
using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Services
{
	public class UserService : UserManager<UserDAO>
	{
		public UserService(IUserStore<UserDAO> store,
					 IOptions<IdentityOptions> optionsAccessor,
					 IPasswordHasher<UserDAO> passwordHasher,
					 IEnumerable<IUserValidator<UserDAO>> userValidators,
					 IEnumerable<IPasswordValidator<UserDAO>> passwordValidators,
					 ILookupNormalizer keyNormalizer,
					 IdentityErrorDescriber errors,
					 IServiceProvider services,
					 ILogger<UserManager<UserDAO>> logger) 
			: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		{
		}

		public async Task<Result<User>> RegisterUserAsync(string userName, string email, string rawPassword, CancellationToken cancellationToken = default)
		{
			var result = await CreateAsync(new UserDAO { UserName = userName, Email = email }, rawPassword);

			if (result.Succeeded == false)
				return Result.CreateError<User>(result.Errors.Select(e => e.Description));

			var user = await FindByEmailAsync(email);

			if (user is null)
				return Result.CreateError<User>(new string[] { "Something when creating the user - Created user could not be found" });

			return Result.CreateSuccess(
				new User(user.Id, user.UserName, user.Email, user.PasswordHash)
			);

		}
	}
}
