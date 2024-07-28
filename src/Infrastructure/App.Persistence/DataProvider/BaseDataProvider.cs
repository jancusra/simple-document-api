using System;
using System.Threading.Tasks;
using App.Domain;

namespace App.Persistence.DataProvider
{
    public abstract class BaseDataProvider
    {
        /// <summary>
        /// Factory method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : BaseEntity;

        /// <summary>
        /// Factory method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public abstract Task InsertDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Factory method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
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
