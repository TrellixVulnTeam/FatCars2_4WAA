using FatCars.Repository.Dapper.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FatCars.Repository.Dapper
{
	public class DatabaseConfig : IDatabaseConfig
	{
		public string ConnectionString { get; set; }

		public DatabaseConfig(IConfiguration config)
		{
			this.ConnectionString = config["Database:ConnectionString"];
		}
	}
}
