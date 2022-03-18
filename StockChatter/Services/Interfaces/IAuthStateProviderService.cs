namespace StockChatter.Services.Interfaces
{
	public interface IAuthStateProviderService<TCredential>
	{
		Task NotifyUserAuthentication(TCredential credential);
		void NotifyUserLogout();
	}
}
