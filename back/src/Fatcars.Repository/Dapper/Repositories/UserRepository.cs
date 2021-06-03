using System;
using System.Threading.Tasks;
using FatCars.Repository.Dapper;
using FatCars.Repository.Dapper.Interfaces;
using FatCars.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace FatCars.Repository.Dapper.Repositories
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
