using Microsoft.EntityFrameworkCore;
using StockChatter.API.Infrastructure.Database;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Repositories.Interfaces;

namespace StockChatter.API.Infrastructure.Repositories
{
    public class MessagesRepository : IMessagesRepository
    {
        private readonly DbSet<MessageDAO> _messages;

        public MessagesRepository(StockChatterContext dbContext)
        {
            _messages = dbContext.Messages;
        }

        public Task Add(MessageDAO message, CancellationToken cancellationToken = default)
        {
            _messages.Add(message);
            return Task.CompletedTask;
        }
    }
}
