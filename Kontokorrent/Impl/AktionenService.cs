using Kontokorrent.Impl.EFV2;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kontokorrent.Impl
{
    public class AktionenService : IAktionenService
    {
        private readonly KontokorrentV2Context kontokorrentV2Context;
        private readonly IKontokorrentsService kontokorrentsService;
        private static ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public AktionenService(KontokorrentV2Context kontokorrentV2Context, IKontokorrentsService kontokorrentsService)
        {
            this.kontokorrentV2Context = kontokorrentV2Context;
            this.kontokorrentsService = kontokorrentsService;
        }
        public async Task<Models.Aktion[]> Auflisten(BenutzerID benutzer, string kontokorrentId, int? ab)
        {
            if (!await kontokorrentsService.HasAccess(benutzer, kontokorrentId))
            {
                return null;
            }
            return await kontokorrentV2Context.Aktionen
                .Where(v => v.KontokorrentId == kontokorrentId)
                .OrderBy(v => v.LaufendeNummer)
                .Include(v => v.Bezahlung)
                .Include(v => v.Bezahlung.BezahlendePerson)
                .Include(v => v.Bezahlung.Emfpaenger)
                .ThenInclude(v => v.Empfaenger)
                .Where(v => ab == null || v.LaufendeNummer > ab)
                .Select(v => new Models.Aktion()
                {
                    LaufendeNummer = v.LaufendeNummer,
                    BearbeiteteBezahlungId = v.BearbeiteteBezahlungId,
                    Bezahlung = v.Bezahlung != null ? new Models.Bezahlung()
                    {
                        Beschreibung = v.Bezahlung.Beschreibung,
                        BezahlendePerson = new Models.Person()
                        {
                            Id = v.Bezahlung.BezahlendePersonId,
                            Name = v.Bezahlung.BezahlendePerson.Name
                        },
                        Id = v.Bezahlung.Id,
                        Empfaenger = v.Bezahlung.Emfpaenger.Select(d => new Models.Person() { 
                            Id = d.Empfaenger.Id,
                            Name = d.Empfaenger.Name
                        } ).ToArray(),
                        Wert = v.Bezahlung.Wert,
                        Zeitpunkt = new DateTimeOffset(v.Bezahlung.Zeitpunkt, TimeSpan.Zero)
                    } : null,
                    GeloeschteBezahlungId = v.GeloeschteBezahlungId
                }).ToArrayAsync();
        }
        private async Task<int> NeueLaufNummer(string kontokorrentId)
        {
            var k = await kontokorrentV2Context.Kontokorrent.Where(v => v.Id == kontokorrentId).SingleAsync();
            k.LaufendeNummer++;
            return k.LaufendeNummer;
        }

        public async Task<Models.Aktion> BezahlungBearbeiten(BenutzerID? benutzer, string kontokorrentId, string id, GeaenderteBezahlung request)
        {
            if (!benutzer.HasValue || !await kontokorrentsService.HasAccess(benutzer.Value, kontokorrentId))
            {
                return null;
            }
            var payment = kontokorrentV2Context.Aktionen.Where(v => v.KontokorrentId == kontokorrentId && v.BezahlungId == id)
                .Select(v => v.Bezahlung)
                .Include(v => v.Emfpaenger)
                .ThenInclude(v => v.Empfaenger)
                .SingleOrDefault();
            if (null == payment)
            {
                return null;
            }
            await CheckEditierbar(id);
            await CheckPersons(request.EmpfaengerIds, kontokorrentId);
            var aktion = new EFV2.Aktion()
            {
                KontokorrentId = kontokorrentId,
                BearbeiteteBezahlungId = id,
                Bezahlung = new EFV2.Bezahlung()
                {
                    Id = Guid.NewGuid().ToString(),
                    Beschreibung = request.Beschreibung,
                    BezahlendePersonId = payment.BezahlendePersonId,
                    Emfpaenger = request.EmpfaengerIds.Select(p => new EmfpaengerInBezahlung()
                    {
                        EmpfaengerId = p
                    }).ToList(),
                    Wert = request.Wert,
                    Zeitpunkt = request.Zeitpunkt.UtcDateTime,
                },
                LaufendeNummer = await NeueLaufNummer(kontokorrentId)
            };
            await kontokorrentV2Context.SaveChangesAsync();
            return new Models.Aktion()
            {
                GeloeschteBezahlungId = null,
                BearbeiteteBezahlungId = aktion.BearbeiteteBezahlungId,
                Bezahlung = MapBezahlung(aktion.Bezahlung),
                LaufendeNummer = aktion.LaufendeNummer
            };
        }

        public async Task<Models.Aktion> BezahlungHinzufuegen(BenutzerID? benutzer, string kontokorrentId, NeueBezahlung bezahlung)
        {
            if (!benutzer.HasValue || !await kontokorrentsService.HasAccess(benutzer.Value, kontokorrentId))
            {
                return null;
            }
            var personIds = bezahlung.EmpfaengerIds.Union(new[] { bezahlung.BezahlendePersonId });
            await CheckPersons(personIds, kontokorrentId);
            var sem = locks.GetOrAdd(kontokorrentId, new SemaphoreSlim(1));
            try
            {
                await sem.WaitAsync();

                var a = new EFV2.Aktion()
                {
                    KontokorrentId = kontokorrentId,
                    LaufendeNummer = await NeueLaufNummer(kontokorrentId),
                    Bezahlung = new EFV2.Bezahlung()
                    {
                        Beschreibung = bezahlung.Beschreibung,
                        BezahlendePersonId = bezahlung.BezahlendePersonId,
                        Emfpaenger = bezahlung.EmpfaengerIds.Select(p => new EmfpaengerInBezahlung()
                        {
                            EmpfaengerId = p
                        }).ToList(),
                        Id = Guid.NewGuid().ToString(),
                        Wert = bezahlung.Wert,
                        Zeitpunkt = bezahlung.Zeitpunkt.UtcDateTime,
                    }
                };
                kontokorrentV2Context.Aktionen.Add(a);
                await kontokorrentV2Context.SaveChangesAsync();
                var b = await kontokorrentV2Context.Bezahlung
                    .Include(v => v.BezahlendePerson)
                    .Include(v => v.Emfpaenger)
                    .ThenInclude(v => v.Empfaenger)
                    .Where(b => b.Id == a.Bezahlung.Id).SingleAsync();
                return new Models.Aktion()
                {
                    GeloeschteBezahlungId = null,
                    BearbeiteteBezahlungId = null,
                    Bezahlung = MapBezahlung(b),
                    LaufendeNummer = a.LaufendeNummer
                };
            }
            finally
            {
                sem.Release();
            }
        }

        private async Task CheckPersons(IEnumerable<string> personIds, string kontokorrentId)
        {
            var tasks = personIds.Select(async id => (await kontokorrentV2Context.Person.AnyAsync(v => v.Id == id && v.KontokorrentId == kontokorrentId))).ToArray();
            if (!(await Task.WhenAll(tasks)).All(d => d))
            {
                throw new PersonExistiertNichtException();
            }
        }

        private static Models.Bezahlung MapBezahlung(EFV2.Bezahlung b)
        {
            return new Models.Bezahlung()
            {
                Beschreibung = b.Beschreibung,
                BezahlendePerson = new Models.Person()
                {
                    Id = b.BezahlendePerson.Id,
                    Name = b.BezahlendePerson.Name
                },
                Id = b.Id,
                Empfaenger = b.Emfpaenger.Select(e => new Models.Person()
                {
                    Id = e.Empfaenger.Id,
                    Name = e.Empfaenger.Name
                }).ToArray(),
                Wert = b.Wert,
                Zeitpunkt = b.Zeitpunkt
            };
        }

        public async Task<Models.Aktion> BezahlungLoeschen(BenutzerID? benutzer, string kontokorrentId, string id)
        {
            if (!benutzer.HasValue || !await kontokorrentsService.HasAccess(benutzer.Value, kontokorrentId))
            {
                return null;
            }
            var b = await kontokorrentV2Context.Aktionen.Where(p => p.KontokorrentId == kontokorrentId && p.BezahlungId == id).SingleOrDefaultAsync();
            if (null == b)
            {
                return null;
            }
            await CheckEditierbar(id);
            var sem = locks.GetOrAdd(b.KontokorrentId, new SemaphoreSlim(1));
            try
            {
                await sem.WaitAsync();
                var loeschung = new EFV2.Aktion()
                {
                    KontokorrentId = kontokorrentId,
                    GeloeschteBezahlungId = id,
                    LaufendeNummer = await NeueLaufNummer(b.KontokorrentId)
                };
                kontokorrentV2Context.Aktionen.Add(loeschung);
                await kontokorrentV2Context.SaveChangesAsync();
                return new Models.Aktion()
                {
                    LaufendeNummer = loeschung.LaufendeNummer,
                    GeloeschteBezahlungId = loeschung.GeloeschteBezahlungId
                };

            }
            finally
            {
                sem.Release();
            }
        }

        private async Task CheckEditierbar(string id)
        {
            if (await kontokorrentV2Context.Aktionen.Where(v => v.GeloeschteBezahlungId == id || v.BearbeiteteBezahlungId == id).AnyAsync())
            {
                throw new BezahlungNichtEditierbarException();
            }
        }
    }
}
