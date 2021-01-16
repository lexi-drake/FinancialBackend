using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore;

namespace WebService
{
    public class Program
    {
        private const string APPSETTINGS = "appsettings.json";
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    if (File.Exists(APPSETTINGS))
                    {
                        // We can load config from appsettings locally
                        config.AddJsonFile(APPSETTINGS);
                    }
                    else
                    {
                        // When we deploy, we want to use environment variables insead
                        // of appsettings.json
                        config.AddEnvironmentVariables();
                    }
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                })
                .UseStartup<Startup>();
    }
}
