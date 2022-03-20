using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Repositories.Interfaces
{
    public interface IMessagesRepository
    {
        IQueryable<MessageDAO> Messages { get; }

        Task Add(MessageDAO message, CancellationToken cancellationToken = default);
    }
}