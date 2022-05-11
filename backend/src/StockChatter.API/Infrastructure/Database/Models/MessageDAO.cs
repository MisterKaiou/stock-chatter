namespace StockChatter.API.Infrastructure.Database.Models
{
    public class MessageDAO
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
