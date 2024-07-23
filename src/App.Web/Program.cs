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
    }
}
