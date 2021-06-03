using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FatCars.Repository;
using FatCars.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FatCars.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LoginController : ControllerBase
	{
		private readonly ILogger<LoginController> _log;
		private readonly DataContext _context;
		private readonly IConfiguration _config;

		public LoginController(DataContext context, ILogger<LoginController> log, IConfiguration config)
		{
			this._config = config;
			this._context = context;
			this._log = log;
		}

		[HttpPost]
		public async Task<IActionResult> Login(
			[FromHeader] string Username,
			[FromHeader] string Password)
		{
#if DEBUG
			var _user = new Users { Login = "TESTE", Name = "NAME", Password = "PASSWORD", Role = "ROLE", UserID = 3 };
			var _token = GenerateToken(_user);
			return Ok(new
			{
				User = _user.Login,
				Token = _token
			});
#endif
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == Username && u.Password == Password);

			if (user is null)
				return Unauthorized();

			var token = GenerateToken(user);
			return Ok(new
			{
				User = user.Login,
				token = token
			});
		}

		private string GenerateToken(Users user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Role, user.Role)
			};
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

			var token =
				new JwtSecurityToken(
					issuer: _config["Jwt:Issuer"],
					audience: _config["Jwt:Audience"],
					expires: DateTime.Now.AddMinutes(120),
					signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));

			var tokenHandler = new JwtSecurityTokenHandler();
			var stringToken = tokenHandler.WriteToken(token);
			return stringToken;
		}

	}
}
