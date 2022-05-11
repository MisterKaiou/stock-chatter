using StockChatter.API.Infrastructure.Database;
using StockChatter.API.Infrastructure.Repositories.Interfaces;

namespace StockChatter.API.Infrastructure.Repositories
{
	public class UnitOfWork : IUoW
	{
		public UnitOfWork(StockChatterContext context) => _context = context;

		private readonly StockChatterContext _context;

		private IMessagesRepository? _messagesRepository;
		public IMessagesRepository MessagesRepository
		{
			get
			{
				if (_messagesRepository == null)
					_messagesRepository = new MessagesRepository(_context);

				return _messagesRepository;
			}
		}

		private IUsersRepository? _usersRepository;
		public IUsersRepository UsersRepository
		{
			get
			{
				if (_usersRepository == null)
					_usersRepository = new UsersRepository(_context);

				return _usersRepository;
			}
		}

		public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
