using Kontokorrent.Impl.EFV2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace Kontokorrent
{
    public class Program
    {
        private static async Task MigrateDatabase(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<KontokorrentV2Context>();
            await context.Database.MigrateAsync();
            var kontokorrents = await context.Kontokorrent.ToArrayAsync();
            foreach (var k in kontokorrents)
            {
                if (k.OeffentlicherName != null && k.OeffentlicherName != k.OeffentlicherName.ToLower())
                {
                    k.OeffentlicherName = k.OeffentlicherName.ToLower();
                }
            }
            await context.SaveChangesAsync();
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
