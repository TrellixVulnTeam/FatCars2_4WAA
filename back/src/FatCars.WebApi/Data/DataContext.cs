using Microsoft.EntityFrameworkCore;
using FatCars.WebApi.Models;

namespace FatCars.WebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Users> Users { get; set; }
    }
}