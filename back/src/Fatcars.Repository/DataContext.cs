using Microsoft.EntityFrameworkCore;
using FatCars.Domain;
using Microsoft.Extensions.Configuration;

namespace FatCars.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options, IConfiguration config) : base(options) { }
        public DbSet<Users> Users { get; set; }
    }
}