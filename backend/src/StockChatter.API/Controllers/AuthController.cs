using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using StockChatter.API.Infrastructure.Database.Models;
using StockChatter.Shared.Models.Auth;
using StockChatter.Shared.Models.Common;
using System.Net;

namespace StockChatter.API.Controllers
{
    [ApiController]
	[Route("[controller]")]
	[ProducesErrorResponseType(typeof(ErrorModel))]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<UserDAO> _userManager;
		private readonly ILogger<AuthController> _logger;

		public AuthController(UserManager<UserDAO> userManager, ILogger<AuthController> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
		{
			var registerResult = await _userManager.CreateAsync(new UserDAO { Email = request.Email, UserName = request.UserName }, request.Password);

			if (registerResult.Succeeded == false)
				return BadRequest(new ErrorModel { Errors = registerResult.Errors.Select(e => e.Description) });

			return NoContent();
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromServices] JwtTokenSettings tokenSettings)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);

			if (user is null)
				return NotFound(new ErrorModel { Errors = new[] { "User not found" }});

			if (await _userManager.CheckPasswordAsync(user, request.Password) == false)
				return base.StatusCode((int)HttpStatusCode.Unauthorized, new ErrorModel { Errors = new[] { "Wrong password" } });

			return Ok(GenerateToken(user, tokenSettings));
		}

#if DEBUG

		[HttpGet("verify")]
		[Authorize]
		public IActionResult VefifyCredentials() => NoContent();
#endif

		private static string GenerateToken(UserDAO user, JwtTokenSettings tokenSettings)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.Name, user.UserName),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim(JwtRegisteredClaimNames.Iss, tokenSettings.Issuer),
				new Claim(JwtRegisteredClaimNames.Aud, tokenSettings.Audience),
				new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
				new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds().ToString()),
			};

			var header = new JwtHeader(
				new SigningCredentials(
					new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSettings.SecretKey)),
					SecurityAlgorithms.HmacSha256)
			);

			var payload = new JwtPayload(claims);
			var token = new JwtSecurityToken(header, payload);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}