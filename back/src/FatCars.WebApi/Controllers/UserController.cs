using System.Linq;
using System.Threading.Tasks;
using FatCars.Domain;
using FatCars.Repository;
using FatCars.Repository.Dapper.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FatCars.Webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _log;
        public readonly DataContext _context;
        private readonly IUserRepository _dapperRepository;

        public UserController(DataContext context, ILogger<UserController> log, IUserRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
            _log = log;
            _context = context;
        }

        //Get All Users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                _log.LogInformation($"Initializing {nameof(GetAll)}");
                var result = await _context.Users.ToListAsync();

                return result.Any() ? Ok(result) : NoContent();
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "");
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Falha no Banco - {ex.Message}");
            }

        }

        //Get By Id Users
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _log.LogInformation($"Initializing {nameof(GetById)}");

                //FROM ENTITY
                //var result = await _context.Users.FirstOrDefaultAsync(x => x.UserID == id);

                //FROM DAPPER
                var result = await _dapperRepository.GetById(id);

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "");
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Falha no Banco - {ex.Message}");
            }
        }


        [HttpPost]
        [Authorize]
        public IActionResult SaveUser(Users user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Usuario Invalido");

                _context.Users.Add(user);

                _log.LogInformation($"Usuario Inserido: user");

                _context.SaveChanges();
                return Ok();

            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "");
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Falha no Banco - {ex.Message}");
            }
        }


        [HttpPut]
        [Authorize]
        public IActionResult UpdateUser(Users user)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest("Usuario Invalido"); }

                var exist = _dapperRepository.CheckUser(user.Id);

                if (exist) {
                    _context.Users.Update(user);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest("Usuario não existe");
                }
              
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "");
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Falha no Banco - {ex.Message}");
            }
        }

        //[HttpDelete]
        //public IActionResult Delete(int id)
        //{

        //	var user = await GetById(id);
        //	_context.SaveChanges();
        //	return Ok(retUser);

        //}



        /* 
		 //Get All Users
		 [HttpGet]
		 public ActionResult<IEnumerable<Users>> GetAll(){


			 return ;
		 }

		 //Get By Id Users
		 [HttpGet("{id}")]
		 public ActionResult<Users> GetById(int USerId){


			 return Ok;
		 }

		 Post Users
		 [HttpPost]
		 public Users SaveUser(Users users){


			 return Ok;
		 }*/
    }
}
/*using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApiSample.Controllers
{
    #region snippet_Class
    [ApiController]
    [Route("api/[controller]")]
    public class ConsumesController : ControllerBase
    {

        */