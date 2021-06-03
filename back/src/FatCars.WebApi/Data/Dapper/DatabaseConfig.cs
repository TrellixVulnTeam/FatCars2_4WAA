using FatCars.WebApi.Data.Dapper.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FatCars.WebApi.Data.Dapper
{
	public class DatabaseConfig : IDatabaseConfig
	{
		public string ConnectionString { get; set; }

		public DatabaseConfig(IConfiguration config)
		{
			this.ConnectionString = config["Dapper:ConnectionString"];
		}
	}
}
