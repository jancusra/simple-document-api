using App.Domain;
using System.Threading.Tasks;
using System;

namespace App.Persistence.DataProvider
{
    public partial interface IDataProvider
    {
        /// <summary>
        /// Get document/entity by ID
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="id">document unique identifier</param>
        /// <returns>final entity</returns>
        public Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : BaseEntity;

        /// <summary>
        /// Insert document/entity
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        /// <summary>
        /// Update document/entity
        /// </summary>
        /// <typeparam name="TEntity">document type</typeparam>
        /// <param name="entity">specific document with identifier</param>
        Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    }
}
