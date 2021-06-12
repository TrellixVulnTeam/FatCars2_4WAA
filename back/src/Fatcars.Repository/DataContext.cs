using Microsoft.EntityFrameworkCore;
using FatCars.Domain;
using Microsoft.Extensions.Configuration;

namespace FatCars.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options, IConfiguration config) : base(options) { }
        public DbSet<Users> Users { get; set; }
        public DbSet<Email> Emails { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Recurso usado para indicar rela��es de muitos para muitos
            //modelBuilder.Entity<>()
            //            .HasKey(x => new {a, b});
        }*/
    }
}