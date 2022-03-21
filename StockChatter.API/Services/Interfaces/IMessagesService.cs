using StockChatter.API.Domain.Entitites.Messages;

namespace StockChatter.API.Services.Interfaces
{
	public interface IMessagesService
	{
		Task<IEnumerable<Message>> FetchMessagesStartingFromAsync(DateTime date);
		Task PostMessageAsync(Message message, CancellationToken cancellationToken = default);
	}
}