namespace StockChatter.API.Domain.Entitites.Messages
{
    public record Message
    {
        public Guid SenderIdentifier { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public PastDate SentAt { get; set; }

        public Message(Guid senderIdentifier, string senderName, string content, PastDate sentAt)
        {
            SenderIdentifier = senderIdentifier;
            Content = content;
            SentAt = sentAt;
            SenderName = senderName;
        }
    }
}
