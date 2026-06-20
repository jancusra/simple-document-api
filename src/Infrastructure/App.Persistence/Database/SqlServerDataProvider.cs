using System;
using System.Threading.Tasks;
using App.Domain.Exceptions;
using App.Persistence.DataProvider;
using Microsoft.Data.SqlClient;
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
            return await _appDbContext.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public override async Task AddDocumentAsync<TEntity>(TEntity entity)
        {
            var existing = await GetDocumentByIdAsync<TEntity>(entity.Id);

            if (existing != null)
            {
                throw new EntityEntryAlreadyExistsException(typeof(TEntity).Name, entity.Id);
            }

            _appDbContext.Set<TEntity>().Add(entity);

            try
            {
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
            {
                // A concurrent insert won the race between the existence check above and the save.
                throw new EntityEntryAlreadyExistsException(typeof(TEntity).Name, entity.Id);
            }
        }

        public override async Task UpdateDocumentAsync<TEntity>(TEntity entity)
        {
            await EnsureEntityExistsAsync<TEntity>(entity.Id);

            _appDbContext.Set<TEntity>().Update(entity);
            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Determine whether a failed save was caused by a duplicate key (unique constraint) violation
        /// </summary>
        /// <param name="exception">EF save exception</param>
        /// <returns>true when the inner SQL error is a unique key violation</returns>
        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            // 2627 = unique constraint (primary key) violation, 2601 = duplicate key in a unique index
            return exception.InnerException is SqlException sqlException
                && (sqlException.Number == 2627 || sqlException.Number == 2601);
        }
    }
}
