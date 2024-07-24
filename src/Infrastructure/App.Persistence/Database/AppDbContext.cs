using Microsoft.EntityFrameworkCore;
using App.Domain.Entities;

namespace App.Persistence.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=AppApi;User ID=sa;Password=local456***;TrustServerCertificate=true");
        }
    }
}
