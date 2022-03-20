using StockChatter.API.Domain.Entitites.Messages;
using StockChatter.API.Infrastructure.Repositories.Interfaces;
using StockChatter.API.Infrastructure.Services.Interfaces;

namespace StockChatter.API.Infrastructure.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IMessagesRepository _repository;

        public MessagesService(IMessagesRepository repository)
        {
            _repository = repository;
        }

        public Task PostMessage(Message message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
