using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using App.Domain.Entities;
using App.Domain;

namespace App.Persistence.Database
{
    public partial class AppDbContext : DbContext, IAppDbContext
    {
        private readonly IOptions<DatabaseConfiguration> _databaseConfiguration;

        public AppDbContext(IOptions<DatabaseConfiguration> databaseConfiguration)
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
