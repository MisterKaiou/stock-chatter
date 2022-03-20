using StockChatter.API.Domain.Entitites.Messages;

namespace StockChatter.API.Infrastructure.Services.Interfaces
{
    public interface IMessagesService
    {
        Task PostMessageAsync(Message message, CancellationToken cancellationToken = default);
    }
}