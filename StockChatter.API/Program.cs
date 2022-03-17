using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockChatter.API.Infrastructure.Database;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.API.Infrastructure.Services;
using Serilog;
using System.Text;
using StockChatter.API;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(ctx.Configuration));

builder.Host.ConfigureServices((ctx, services) =>
{
	services.AddControllers();
	services.AddEndpointsApiExplorer();

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
		opt.Password.RequiredUniqueChars = 3;
		opt.Password.RequireDigit = false;
		opt.Password.RequireUppercase = false;
		opt.Password.RequireNonAlphanumeric = false;
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
		opt.SaveToken = false;
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
	});

	#endregion
});

var app = builder.Build();

app.UseCors(opt => opt
		.AllowAnyOrigin()
		.AllowAnyHeader()
		.AllowAnyMethod()
);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
	await scope.ServiceProvider.GetRequiredService<StockChatterContext>().Database.EnsureCreatedAsync();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseSerilogRequestLogging();

app.Run();
