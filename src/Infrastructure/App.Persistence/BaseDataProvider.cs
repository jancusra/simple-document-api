using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using App.Domain;

namespace App.Persistence
{
    public abstract class BaseDataProvider
    {
        public abstract Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : DocumentEntity;

        public abstract Task InsertDocumentAsync<TEntity>(TEntity entity) where TEntity : DocumentEntity;

        public abstract Task ModifyDocumentAsync<TEntity>(TEntity entity) where TEntity : DocumentEntity;

        public abstract Task<IList<TEntity>> GetAllDocumentsAsync<TEntity>() where TEntity : DocumentEntity;

        public virtual async Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : DocumentEntity
        {
            var doc = await GetDocumentByIdAsync<TEntity>(entity.Id);

            if (doc == null)
            {
                await InsertDocumentAsync<TEntity>(entity);
            }
        }

        public virtual async Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : DocumentEntity
        {
            var doc = await GetDocumentByIdAsync<TEntity>(entity.Id);

            if (doc != null)
            {
                await ModifyDocumentAsync<TEntity>(entity);
            }
        }
    }
}
