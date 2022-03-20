namespace StockChatter.API.Domain.Entitites.Messages
{
    public struct PastDate
    {
        private DateTime _date;

        public PastDate(DateTime time)
        {
            if (time > DateTime.Now)
                throw new ArgumentException("Date must be a past date.", nameof(time));

            _date = time;
        }

        public static implicit operator DateTime(PastDate pastDate) => pastDate._date;
        public static implicit operator PastDate(DateTime date) => new(date);
    }
}
