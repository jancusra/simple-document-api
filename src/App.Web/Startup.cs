using System.IO.Compression;
using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Alphacloud.MessagePack.AspNetCore.Formatters;
using App.Persistence;
using App.Persistence.Database;
using App.Persistence.DataProvider;
using App.Domain.ErrorMiddleware;

namespace App.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">collection of services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            //see https://docs.microsoft.com/dotnet/framework/network-programming/tls
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;

            services.Configure<StorageTypeConfig>(_configuration);
            services.Configure<DatabaseConfig>(_configuration);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IDataProviderManager, DataProviderManager>()
                    .AddTransient(serviceProvider =>
                        serviceProvider.GetRequiredService<IDataProviderManager>().DataProvider);

            // Optional "CacheSizeLimit" config key bounds the in-memory store (LRU eviction).
            // When absent the limit is null and the store keeps every entry (unbounded).
            var cacheSizeLimit = _configuration.GetValue<long?>("CacheSizeLimit");
            services.AddMemoryCache(options => options.SizeLimit = cacheSizeLimit);
            services.AddDbContext<IAppDbContext, AppDbContext>((serviceProvider, options) =>
            {
                var databaseConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>();
                options.UseSqlServer(databaseConfig.Value.ConnectionString);
            });

            services.AddMessagePack();

            // The service is read-heavy, so compress responses to cut bandwidth and latency.
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);

            services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
                .AddXmlDataContractSerializerFormatters()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddHealthChecks();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">application builder</param>
        /// <param name="env">web host environment</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseResponseCompression();

            app.UseRouting();
            app.UseMiddleware<ErrorWrappingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}
