using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using StockChatter;
using StockChatter.Services.Interfaces;
using StockChatter.Services;
using StockChatter.HubClients;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<StockChatterClient>(svcs => new StockChatterClient(
		new HttpClient
		{ 
			BaseAddress = new Uri(svcs.GetRequiredService<IConfiguration>().GetValue<string>("ApiBaseUri")) 
		},
		svcs.GetRequiredService<ILocalStorageService>()
	)
);

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProviderService>();
builder.Services.AddScoped<IJwtStateProviderService, JwtAuthStateProviderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ChatRoomHubClient>();

await builder.Build().RunAsync();
