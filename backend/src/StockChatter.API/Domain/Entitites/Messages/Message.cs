namespace StockChatter.API.Domain.Entitites.Messages
{
    public record Message(Guid SenderIdentifier, string SenderName, string Content, PastDate SentAt);
}
