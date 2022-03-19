using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using StockChatter.API.Hubs;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Database;
using StockChatter.API.Infrastructure.Providers;
using StockChatter.API.Infrastructure.Services;
using StockChatter.API;
using System.Net.Mime;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Host.ConfigureServices((ctx, services) =>
{
	services.AddControllers();
	services.AddEndpointsApiExplorer();
	services.AddCors();
	
	services.AddResponseCompression(opt =>
	{
		opt.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
			new[] { MediaTypeNames.Application.Octet }
		);
	});

	#region Swagger Setup

	services.AddSwaggerGen(opt =>
	{
		opt.SwaggerDoc("v1", new OpenApiInfo
		{
			Title = "StockChatty API",
			Version = "v1",
			Description = "API for the application StockChatty"
		});

		opt.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
		{
			Type = SecuritySchemeType.Http,
			Scheme = JwtBearerDefaults.AuthenticationScheme,
			BearerFormat = "JWT",
			Description = "JWT Authorization header using the Bearer scheme"
		});

		opt.AddSecurityRequirement(new OpenApiSecurityRequirement
		{
			{
				new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = JwtBearerDefaults.AuthenticationScheme
					}
				},
				Array.Empty<string>()
			}
		});
	});

	#endregion

	#region Application Services Configuration

	builder.Services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

	#endregion

	#region Infrastructure Configuration

	services.AddAutoMapper(typeof(Program).Assembly);

	services
		.AddDbContext<StockChatterContext>(opt => opt
			.UseSqlServer(ctx.Configuration.GetConnectionString("mainDatabase"))
		);

	#endregion

	#region Identity Configuration

	services.AddSingleton<IPasswordHasher<UserDAO>, PasswordHasherService>();

	services.AddIdentity<UserDAO, IdentityRole<Guid>>(opt =>
	{
		opt.User.RequireUniqueEmail = true;

#if DEBUG
		// Makes it easier to create disposable credentials.
		opt.Password.RequiredLength = 3;
		opt.Password.RequiredUniqueChars = 3;
		opt.Password.RequireDigit = false;
		opt.Password.RequireUppercase = false;
		opt.Password.RequireNonAlphanumeric = false;
		opt.Password.RequireLowercase = false;
#else
		opt.Password.RequiredLength = 8;
		opt.Password.RequiredUniqueChars = 3;
#endif
	})
	.AddEntityFrameworkStores<StockChatterContext>();

	#endregion

	#region Authentication Setup

	var jwtTokenSettings =
		ctx.Configuration.GetSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>()
		?? throw new ArgumentNullException(nameof(JwtTokenSettings), "Failed to load JWT Token settings from configuration");
	var key = Encoding.ASCII.GetBytes(jwtTokenSettings.SecretKey);

	services.AddSingleton<JwtTokenSettings>(jwtTokenSettings);

	services.AddAuthentication(opt =>
	{
		opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(opt =>
	{
		opt.RequireHttpsMetadata = false;
		opt.SaveToken = true;

		opt.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidIssuer = jwtTokenSettings.Issuer,

			ValidateAudience = true,
			ValidAudience = jwtTokenSettings.Audience,

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(key),

			RequireExpirationTime = true,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};

		// See: https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-6.0#built-in-jwt-authentication
		opt.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				var token = context.Request.Query["access_token"];
				var path = context.HttpContext.Request.Path;

				if(!string.IsNullOrEmpty(token) && path.StartsWithSegments("/chatRoom"))
				{
					context.Token = token;
					context.Request.Headers.Authorization = $"Bearer {token}";
				}

				return Task.CompletedTask;
			}
		};
	});

	#endregion

	services.AddSignalR();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
	await scope.ServiceProvider.GetRequiredService<StockChatterContext>().Database.EnsureCreatedAsync();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors(opt => opt
		.WithOrigins("http://localhost:5000", "https://localhost:5001")
		.AllowAnyHeader()
		.AllowAnyMethod()
		.AllowCredentials()
		.SetIsOriginAllowed(host => true)
);
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatRoomHub>("/chatRoom");
app.MapControllers();

app.UseSerilogRequestLogging();

app.Run();
