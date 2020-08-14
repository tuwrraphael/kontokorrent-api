using Kontokorrent.Impl.EF;
using Kontokorrent.Impl.EFV2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using V2Kontokorrent = Kontokorrent.Impl.EFV2.Kontokorrent;

namespace Kontokorrent
{
    public class Program
    {
        private static async Task MigrateDatabase(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<KontokorrentV2Context>();
            await context.Database.MigrateAsync();
            if (false)
            {
                using var oldContext = serviceScope.ServiceProvider.GetService<KontokorrentContext>();
                var kks = await oldContext.Kontokorrent
                    .Include(v => v.Bezahlungen)
                    .ThenInclude(v => v.Emfpaenger)
                    .Include(v => v.Personen).ToArrayAsync();
                foreach (var kk in kks)
                {
                    var laufendeNummer = 0;
                    var aktionen = new List<Impl.EFV2.Aktion>();
                    foreach (var b in kk.Bezahlungen.OrderBy(v => v.Zeitpunkt))
                    {
                        aktionen.Add(new Aktion()
                        {
                            KontokorrentId = kk.Id,
                            LaufendeNummer = ++laufendeNummer,
                            Bezahlung = new Impl.EFV2.Bezahlung()
                            {
                                Id = b.Id,
                                Beschreibung = b.Beschreibung,
                                BezahlendePersonId = b.BezahlendePersonId,
                                Emfpaenger = b.Emfpaenger.Select(v => new Impl.EFV2.EmfpaengerInBezahlung()
                                {
                                    EmpfaengerId = v.EmpfaengerId
                                }).ToList(),
                                Wert = b.Wert,
                                Zeitpunkt = b.Zeitpunkt
                            }
                        });
                        if (b.Deleted)
                        {
                            aktionen.Add(new Aktion()
                            {
                                KontokorrentId = kk.Id,
                                LaufendeNummer = ++laufendeNummer,
                                GeloeschteBezahlungId = b.Id
                            });
                        }
                    }
                    var migrated = new V2Kontokorrent()
                    {
                        Id = kk.Id,
                        Privat = false,
                        OeffentlicherName = kk.Secret,
                        Personen = kk.Personen.Select(v => new Impl.EFV2.Person()
                        {
                            Id = v.Id,
                            Name = v.Name
                        }).ToList(),
                        Name = kk.Secret,
                        Aktionen = aktionen,
                        LaufendeNummer = laufendeNummer
                    };
                    context.Kontokorrent.Add(migrated);
                }
                await context.SaveChangesAsync();
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
