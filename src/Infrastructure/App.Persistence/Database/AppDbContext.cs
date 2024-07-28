using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using App.Domain.Entities;
using App.Domain;

namespace App.Persistence.Database
{
    /// <summary>
    /// Database context definition
    /// </summary>
    public partial class AppDbContext : DbContext, IAppDbContext
    {
        private readonly IOptions<DatabaseConfig> _databaseConfiguration;

        public AppDbContext(IOptions<DatabaseConfig> databaseConfiguration)
        {
            _databaseConfiguration = databaseConfiguration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_databaseConfiguration.Value.ConnectionString);
        }

        public DbSet<TEntity> Table<TEntity>() where TEntity : BaseEntity
        {
            return Set<TEntity>();
        }

        public DbSet<Document> Documents { get; set; }
    }
}
