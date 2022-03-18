using System.ComponentModel.DataAnnotations;

namespace StockChatter.Shared.Models.Auth
{
    public class LoginRequest
    {
        [EmailAddress, Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }
}
