using System;
using Microsoft.Extensions.Caching.Memory;

namespace App.Persistence
{
    /// <summary>
    /// Class for methods managing data providers
    /// </summary>
    public partial class DataProviderManager : IDataProviderManager
    {
        private readonly IMemoryCache _memoryCache;

        public DataProviderManager(
            IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Get configured data manager
        /// </summary>
        /// <returns>data provider</returns>
        /// <exception cref="Exception"></exception>
        public IDataProvider GetDataProvider()
        {
            return new MemoryDataProvider(_memoryCache);

            /*var databaseType = DatabaseSettingsManager.GetSettings().DataProvider;

            return databaseType switch
            {
                DatabaseType.SqlServer => new MsSqlDataProvider(),
                DatabaseType.MySql => new MySqlDataProvider(),
                //DataProviderType.PostgreSQL => new PostgreSqlDataProvider(),
                _ => throw new Exception($"Not supported data provider name: '{databaseType}'")
            };*/
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
