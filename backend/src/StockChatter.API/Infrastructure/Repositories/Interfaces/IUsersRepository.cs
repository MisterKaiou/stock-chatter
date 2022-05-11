using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Repositories.Interfaces
{
	public interface IUsersRepository
	{
		IQueryable<UserDAO> Users { get; }
	}
}
