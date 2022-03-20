using StockChatter.API.Domain.Entitites.Messages;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Repositories.Interfaces;
using StockChatter.API.Infrastructure.Services.Interfaces;

namespace StockChatter.API.Infrastructure.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IUoW _uow;

        public MessagesService(IUoW uow)
        {
            _uow = uow;
        }

        public async Task PostMessageAsync(Message message, CancellationToken cancellationToken = default)
        {
            await _uow.MessagesRepository.Add(new MessageDAO
            {
                Id = Guid.NewGuid(),
                SenderId = message.SenderIdentifier,
                Content = message.Content,
                SentAt = message.SentAt
            }, cancellationToken);

            await _uow.SaveChangesAsync();
        }
    }
}
