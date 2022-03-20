using StockChatter.API.Infrastructure.Database;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Repositories.Interfaces;

namespace StockChatter.API.Infrastructure.Repositories
{
	internal class UsersRepository : IUsersRepository
	{
		private readonly StockChatterContext _context;

		public UsersRepository(StockChatterContext context)
		{
			_context = context;
		}

		public IQueryable<UserDAO> Users => _context.Users;
	}
}