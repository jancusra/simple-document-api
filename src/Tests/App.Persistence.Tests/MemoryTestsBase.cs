using System;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace App.Persistence.Tests
{
    /// <summary>
    /// Base class for initializing the tested cache memory
    /// </summary>
    public abstract class MemoryTestsBase : IDisposable
    {
        private readonly IMemoryCache _memoryCache;

        protected MemoryTestsBase()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();

            _memoryCache = serviceProvider.GetService<IMemoryCache>();
        }

        public IMemoryCache MemoryCache
        {
            get
            {
                return _memoryCache;
            }
        }

        public void Dispose() { }
    }
}
