using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using App.Persistence;
using App.Persistence.Database;

namespace App.Web
{
    public class Program
    {
        /// <summary>
        /// The main starting point of the application
        /// </summary>
        /// <param name="args">app arguments</param>
        public static async Task Main(string[] args)
        {
            var webHost = CreateHostBuilder(args).Build();
            DbMigration(webHost.Services);

            await webHost.RunAsync();
        }

        /// <summary>
        /// Creating an application host builder by configuration
        /// </summary>
        /// <param name="args">app arguments</param>
        /// <returns>final host builder</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        /// <summary>
        /// Create database and necessary tables
        /// </summary>
        /// <param name="serviceProvider">built web services</param>
        public static void DbMigration(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var storageType = scope.ServiceProvider
                    .GetRequiredService<IOptions<StorageTypeConfig>>().Value.StorageType;

                // Only the SQL Server storage needs a physical database/table to be created.
                if (storageType == StorageType.SqlServer)
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
