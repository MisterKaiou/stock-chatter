﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Message>> FetchMessagesStartingFrom(DateTime date)
		{
            var messages = await _uow.MessagesRepository.Messages
                .Join(
                    _uow.UsersRepository.Users,
                    m => m.SenderId,
                    u => u.Id,
                    (m, u) => new { m.Id, m.SenderId, m.SentAt, m.Content, u.UserName })
                .Where(m => m.SentAt > date)
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return messages.Select(m => new Message(m.SenderId, m.UserName, m.Content, m.SentAt));
		}
    }
}
