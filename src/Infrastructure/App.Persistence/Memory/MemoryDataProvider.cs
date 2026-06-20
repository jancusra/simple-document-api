using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Caching.Memory;
using App.Domain.Exceptions;
using App.Persistence.DataProvider;

namespace App.Persistence.Memory
{
    /// <summary>
    /// Represents memory cache data provider
    /// </summary>
    public partial class MemoryDataProvider : BaseDataProvider, IDataProvider
    {
        // The memory cache is a singleton shared across all provider instances, so writes are
        // serialized through a shared lock to keep "check then write" atomic under concurrency.
        // Reads go straight to the (thread-safe) cache and are never blocked.
        private static readonly object _writeLock = new object();

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

        public override Task<TEntity> GetDocumentByIdAsync<TEntity>(Guid id)
        {
            var result = _memoryCache.Get(id.ToString().ToLower());

            if (result is TEntity entity)
            {
                return Task.FromResult(entity);
            }

            return Task.FromResult<TEntity>(default);
        }

        public override Task AddDocumentAsync<TEntity>(TEntity entity)
        {
            var key = entity.Id.ToString().ToLower();

            lock (_writeLock)
            {
                if (_memoryCache.TryGetValue(key, out _))
                {
                    throw new EntityEntryAlreadyExistsException(typeof(TEntity).Name, entity.Id);
                }

                _memoryCache.Set(key, entity, _memoryCacheEntryOptions);
            }

            return Task.CompletedTask;
        }

        public override Task UpdateDocumentAsync<TEntity>(TEntity entity)
        {
            var key = entity.Id.ToString().ToLower();

            lock (_writeLock)
            {
                if (!_memoryCache.TryGetValue(key, out _))
                {
                    throw new NonExistingEntityEntryException(typeof(TEntity).Name, entity.Id);
                }

                _memoryCache.Set(key, entity, _memoryCacheEntryOptions);
            }

            return Task.CompletedTask;
        }
    }
}
