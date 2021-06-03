using System.Threading.Tasks;
using FatCars.WebApi.Models;

namespace FatCars.WebApi.Data.Dapper.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<Users> GetById(int UserId);
	}
}