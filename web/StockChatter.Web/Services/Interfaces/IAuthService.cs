using Functional.Result;
using StockChatter.Shared.Models.Auth;

namespace StockChatter.Web.Services.Interfaces
{
    public interface IAuthService
    {
		Task<Result<string>> LoginUserAsync(LoginRequest request);
		Task<Result<Unit>> RegisterUserAsync(RegistrationRequest request);
    }
}
