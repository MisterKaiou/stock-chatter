namespace StockChatter.API.Infrastructure.Repositories.Interfaces
{
    public interface IUoW
    {
        public IMessagesRepository MessagesRepository { get; }

        public IUsersRepository UsersRepository { get; }
        Task SaveChangesAsync();
    }
}
