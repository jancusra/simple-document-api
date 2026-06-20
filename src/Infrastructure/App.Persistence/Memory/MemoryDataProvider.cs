using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using App.Persistence.DataProvider;

namespace App.Persistence.Memory
{
    /// <summary>
    /// Represents memory cache data provider
    /// </summary>
    public partial class MemoryDataProvider : BaseDataProvider, IDataProvider
    {
        private readonly IMemoryCache _memoryCache;

        private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions;

        public MemoryDataProvider(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            // Each entry counts as one unit against the cache size limit (if one is configured).
            // When no limit is configured the size is ignored and entries are kept indefinitely.
            _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1);
        }

        public override async Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id)
        {
            var result = _memoryCache.Get(id.ToString().ToLower());

            if (result != null && result is TEntity)
            {
                return (TEntity)result;
            }

            await Task.FromResult(0);

            return null;
        }

        public override async Task InsertDocumentAsync<TEntity>(TEntity entity)
        {
            _memoryCache.Set(entity.Id.ToString().ToLower(), entity, options: _memoryCacheEntryOptions);

            await Task.FromResult(0);
        }

        public override async Task UpdateDocumentAsync<TEntity>(TEntity entity)
        {
            await EnsureEntityExistsAsync<TEntity>(entity.Id);

            _memoryCache.Set(entity.Id.ToString().ToLower(), entity, options: _memoryCacheEntryOptions);
        }
    }
}
