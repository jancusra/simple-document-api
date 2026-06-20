using Microsoft.EntityFrameworkCore;
using App.Domain.Entities;

namespace App.Persistence.Database
{
    /// <summary>
    /// Database context definition
    /// </summary>
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
    }
}
