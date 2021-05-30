using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FatCars.WebApi.Data;
using FatCars.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FatCars.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        public readonly DataContext _context;
        public UserController(DataContext context)
        {
            _context = context;
        }

        //Get All Users
        [HttpGet]
        public async Task<IActionResult> GetAll(){

            try
            {
                var result = await _context.Users.ToListAsync();
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no Banco");
            }
            
        }

        //Get By Id Users
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id){
            try
            {
                var result = await _context.Users.FirstOrDefaultAsync(x => x.UserID == id);
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Falha no Banco");
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