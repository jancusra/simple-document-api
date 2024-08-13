using System;
using System.Threading.Tasks;
using App.Domain;
using App.Domain.Exceptions;

namespace App.Persistence.DataProvider
{
    /// <summary>
    /// Represents a template for the basic data provider
    /// </summary>
    public abstract class BaseDataProvider
    {
        /// <summary>
        /// Get document/entity by ID
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="id">document unique identifier</param>
        /// <returns>final entity</returns>
        public abstract Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : BaseEntity;

        /// <summary>
        /// Insert document/entity
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        public abstract Task InsertDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Update document/entity
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        public abstract Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Adding a document in a safe way (by unique identifier) or throw exception if entity already exists
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        public virtual async Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            var doc = await GetDocumentByIdAsync<TEntity>(entity.Id);

            if (doc == null)
            {
                await InsertDocumentAsync(entity);
            }
            else
            {
                throw new EntityEntryAlreadyExistsException(typeof(TEntity).Name, entity.Id);
            }
        }
    }
}
