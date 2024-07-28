using System;
using System.Threading.Tasks;
using App.Persistence.DataProvider;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence.Database
{
    /// <summary>
    /// Represents SQL server data provider
    /// </summary>
    public partial class SqlServerDataProvider : BaseDataProvider, IDataProvider
    {
        private readonly AppDbContext _appDbContext;

        public SqlServerDataProvider(
            AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public override async Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id)
        {
            return await _appDbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task InsertDocumentAsync<TEntity>(TEntity entity)
        {
            await _appDbContext.Set<TEntity>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public override async Task UpdateDocumentAsync<TEntity>(TEntity entity)
        {
            _appDbContext.Set<TEntity>().Update(entity);
            await _appDbContext.SaveChangesAsync();
        }
    }
}
