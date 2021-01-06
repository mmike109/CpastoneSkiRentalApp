using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CpastroneSkiRental.Data;
using CpastroneSkiRental.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace CpastroneSkiRental
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();


            var configuration = host.Services.GetService<IConfiguration>();
            var hosting = host.Services.GetService<IWebHostEnvironment>();
          
                var secrets = configuration.GetSection("Secrets").Get<AppSecrets>();
                Dbinitilizer.appSecrets = secrets;
            

            using (var scope = host.Services.CreateScope())
            {
                Dbinitilizer.CreateAdminAndUserRoles(scope.ServiceProvider).Wait();
                host.Run();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
