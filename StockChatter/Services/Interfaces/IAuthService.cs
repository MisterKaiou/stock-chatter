using Functional.Result;
using StockChatter.Shared.Models.Auth;

namespace StockChatter.Services.Interfaces
{
    public interface IAuthService
    {
		Task<Result<string>> LoginUserAsync(LoginRequest request);
		Task<Result<Unit>> RegisterUserAsync(RegistrationRequest request);
    }
}
