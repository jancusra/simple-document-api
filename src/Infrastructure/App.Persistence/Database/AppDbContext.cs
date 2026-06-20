using Microsoft.EntityFrameworkCore;
using App.Domain.Entities;
using App.Domain;

namespace App.Persistence.Database
{
    /// <summary>
    /// Database context definition
    /// </summary>
    public partial class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TEntity> Table<TEntity>() where TEntity : BaseEntity
        {
            return Set<TEntity>();
        }

        public DbSet<Document> Documents { get; set; }
    }
}
