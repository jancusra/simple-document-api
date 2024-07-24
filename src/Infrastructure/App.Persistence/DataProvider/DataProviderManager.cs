using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using App.Persistence.Database;
using App.Persistence.Memory;

namespace App.Persistence.DataProvider
{
    /// <summary>
    /// Class for methods managing data providers
    /// </summary>
    public partial class DataProviderManager : IDataProviderManager
    {
        private readonly IOptions<StorageTypeConfig> _storageType;

        private readonly IMemoryCache _memoryCache;

        private readonly AppDbContext _appDbContext;

        public DataProviderManager(
            IOptions<StorageTypeConfig> storageType,
            IMemoryCache memoryCache,
            AppDbContext appDbContext)
        {
            _storageType = storageType;
            _memoryCache = memoryCache;
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Get configured data manager
        /// </summary>
        /// <returns>data provider</returns>
        /// <exception cref="Exception"></exception>
        public IDataProvider GetDataProvider()
        {
            return _storageType.Value.StorageType switch
            {
                StorageType.Memory => new MemoryDataProvider(_memoryCache),
                StorageType.SqlServer => new SqlServerDataProvider(_appDbContext),
                _ => throw new Exception($"Not supported data provider name: '{_storageType.Value.StorageType}'")
            };
        }

        public IDataProvider DataProvider
        {
            get
            {
                return GetDataProvider();
            }
        }
    }
}
