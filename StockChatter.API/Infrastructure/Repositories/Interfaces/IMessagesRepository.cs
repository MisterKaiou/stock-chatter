using StockChatter.API.Infrastructure.Database.Models;

namespace StockChatter.API.Infrastructure.Repositories.Interfaces
{
    public interface IMessagesRepository
    {
        Task Add(MessageDAO message, CancellationToken cancellationToken = default);
    }
}