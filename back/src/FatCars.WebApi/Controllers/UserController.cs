using System.Threading.Tasks;
using FatCars.WebApi.Data;
using FatCars.WebApi.Data.Dapper.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FatCars.WebApi.Controllers
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
        public async Task<IActionResult> GetAll(){

            try
            {
                _log.LogInformation($"Initializing {nameof(GetAll)}");
                var result = await _context.Users.ToListAsync();
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _log.LogError(ex, "");
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no Banco");
            }
            
        }

        //Get By Id Users
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id){
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
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Falha no Banco - {ex.Message}");
            } 
        }

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

        //Post Users
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