using System.Threading.Tasks;
using FatCars.Domain;

namespace FatCars.Repository.Dapper.Interfaces
{
	public interface IUserRepository
	{
		Task<Users> GetById(int UserId);
		bool CheckUser(int UserId);
	}
}