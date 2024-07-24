using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using App.Persistence.DataProvider;

namespace App.Persistence.Database
{
    public class SqlServerDataProvider : BaseDataProvider, IDataProvider
    {
        public SqlServerDataProvider()
        {
        }

        public override async Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id)
        {
            
            await Task.FromResult(0);

            return null;
        }

        public override async Task InsertDocumentAsync<TEntity>(TEntity entity)
        {
            

            await Task.FromResult(0);
        }

        public override async Task UpdateDocumentAsync<TEntity>(TEntity entity)
        {
            

            await Task.FromResult(0);
        }
    }
}
