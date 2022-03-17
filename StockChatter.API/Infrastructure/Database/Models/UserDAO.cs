using Microsoft.AspNetCore.Identity;

namespace StockChatter.API.Infrastructure.Database.Models
{
    public class UserDAO : IdentityUser<Guid>
    {
    }
}
