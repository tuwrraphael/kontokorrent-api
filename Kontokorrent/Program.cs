using Kontokorrent.Impl.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Kontokorrent
{
    public class Program
    {
        private static async Task MigrateDatabase(IHost host)
        {
            using (var serviceScope = host.Services.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<KontokorrentContext>())
                {
                    await context.Database.MigrateAsync();
                }
            }
        }
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await MigrateDatabase(host);
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
