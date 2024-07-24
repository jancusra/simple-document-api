using App.Domain;
using System.Threading.Tasks;
using System;

namespace App.Persistence.DataProvider
{
    public interface IDataProvider
    {
        public Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id) where TEntity : BaseEntity;

        Task AddDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;

        Task UpdateDocumentAsync<TEntity>(TEntity entity) where TEntity : BaseEntity;
    }
}
