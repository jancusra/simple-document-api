using System.Net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Alphacloud.MessagePack.AspNetCore.Formatters;
using App.Persistence;
using App.Persistence.Database;
using App.Persistence.DataProvider;

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

            services.AddMemoryCache();
            services.AddDbContext<IAppDbContext, AppDbContext>();

            services.AddMessagePack();

            services.AddControllers(options => options.RespectBrowserAcceptHeader = true)
                .AddXmlDataContractSerializerFormatters()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
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
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
