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
        /// Add a document/entity in a safe way (by unique identifier) or throw if it already exists.
        /// Implementations must perform the check and the write atomically.
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        public abstract Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Update an existing document/entity or throw if it does not exist
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        public abstract Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Guard that throws when the entity to be updated does not exist
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="id">document unique identifier</param>
        protected async Task EnsureEntityExistsAsync<TEntity>(Guid id) where TEntity : BaseEntity
        {
            var doc = await GetDocumentByIdAsync<TEntity>(id);

            if (doc == null)
            {
                throw new NonExistingEntityEntryException(typeof(TEntity).Name, id);
            }
        }
    }
}
