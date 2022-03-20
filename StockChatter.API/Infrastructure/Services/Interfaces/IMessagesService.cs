using StockChatter.API.Domain.Entitites.Messages;

namespace StockChatter.API.Infrastructure.Services.Interfaces
{
    public interface IMessagesService
    {
        Task PostMessage(Message message, CancellationToken cancellationToken = default);
    }
}