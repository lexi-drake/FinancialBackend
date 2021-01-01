using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore;
using System.IO;

namespace WebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    try
                    {
                        // We can load config from appsettings locally
                        config.AddJsonFile("appsettings.json");
                    }
                    catch (FileNotFoundException)
                    {
                        // When we deploy, we want to use environment variables insead
                        // of appsettings.json
                        config.AddEnvironmentVariables();
                    }
                }).UseStartup<Startup>();
    }
}
