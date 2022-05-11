namespace StockChatter.Web.Services.Interfaces
{
	public interface IAuthStateProviderService<TCredential>
	{
		Task NotifyUserAuthentication(TCredential credential);
		void NotifyUserLogout();
	}
}
