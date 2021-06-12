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
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using FatCars.Application.Dtos;

namespace FatCars.Webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _log;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;


        public LoginController(
            DataContext context,
            SignInManager<Users> signInManager,
            UserManager<Users> userManager,
            ILogger<LoginController> log,
            IConfiguration config,
            IMapper mapper)
        {
            this._config = config;
            this._context = context;
            this._log = log;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginDto login)
        {


            //#if DEBUG
            //			var _user = new Users { Login = "TESTE", Name = "NAME", Password = "PASSWORD", Role = "ROLE", Id = 3 };
            //			var _token = GenerateToken(_user);
            //			return Ok(new
            //			{
            //				User = _user.Login,
            //				Token = _token
            //			});
            //#endif
            try
            {

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login.Login && u.Password == login.Password);

                if (user is null)
                    return Unauthorized();

                var token = GenerateToken(user);
                return Ok(new
                {
                    User = user.Login,
                    token = token
                });
            }
            catch (Exception ex){ 
                _log.LogError("Erro Login: ", ex);
                return BadRequest();
            }
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
