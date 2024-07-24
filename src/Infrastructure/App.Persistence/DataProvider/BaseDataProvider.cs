using System;
using System.Threading.Tasks;
using App.Domain;

namespace App.Persistence.DataProvider
{
    public abstract class BaseDataProvider
    {
        public abstract Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : BaseEntity;

        public abstract Task InsertDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        public abstract Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        public virtual async Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var doc = await GetDocumentByIdAsync<TEntity>(entity.Id);

            if (doc == null)
            {
                await InsertDocumentAsync(entity);
            }
        }
    }
}
