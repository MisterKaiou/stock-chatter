namespace StockChatter.Shared.Models.Auth
{
    public class RegistrationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
