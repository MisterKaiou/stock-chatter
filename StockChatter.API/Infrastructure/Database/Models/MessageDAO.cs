namespace StockChatter.API.Infrastructure.Database.Models
{
    public class MessageDAO
    {
        public Guid Id { get; set; }
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}
