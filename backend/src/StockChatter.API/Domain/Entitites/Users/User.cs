namespace StockChatter.API.Domain.Entitites.Users
{
    public record User
    {
        public Guid Id { get; set; }
        public string UserName { get; }
        public string Password { get; }
        public string Email { get; }

        public User(string username, string password, string email)
        {
            Id = Guid.NewGuid();
            UserName = username;
            Password = password;
            Email = email;
        }

        public User(Guid id, string username, string password, string email)
        {
            Id = id;
            UserName = username;
            Password = password;
            Email = email;
        }
    }
}
