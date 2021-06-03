using System;
using System.Threading.Tasks;
using Dapper;
using FatCars.WebApi.Data.Dapper.Interfaces;
using FatCars.WebApi.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace FatCars.WebApi.Data.Dapper.Repositories
{
	public class UserRepository : IUserRepository
	{
		private readonly string _connectionString;

		public UserRepository(IDatabaseConfig config)
		{
			this._connectionString = config.ConnectionString;
		}

		public async Task<Users> GetById(int UserId)
		{
			await using var connection = new SqliteConnection(_connectionString);
			await connection.OpenAsync();
			var user = await connection.QueryFirstOrDefaultAsync<Users>($"SELECT * from {nameof(Users)} WHERE {nameof(Users.UserID)} = {UserId};");
			return user ?? throw new Exception("User Not Found!");
		}
	}
}
