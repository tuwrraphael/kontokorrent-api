using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.EF
{
    public class BezahlungRepository : IBezahlungRepository
    {
        private readonly KontokorrentContext kontokorrentContext;
        private static ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim>();

        public BezahlungRepository(KontokorrentContext kontokorrentContext)
        {
            this.kontokorrentContext = kontokorrentContext;
        }

        public async Task<ApiModels.v2.Bezahlung> CreateAsync(NeueBezahlung bezahlung, string kontokorrentId)
        {
            var sem = locks.GetOrAdd(kontokorrentId, new SemaphoreSlim(1));
            try
            {
                await sem.WaitAsync();

                var b = new Bezahlung()
                {
                    Beschreibung = bezahlung.Beschreibung,
                    BezahlendePersonId = bezahlung.BezahlendePerson,
                    Emfpaenger = bezahlung.Empfaenger.Select(p => new EmfpaengerInBezahlung()
                    {
                        EmpfaengerId = p
                    }).ToList(),
                    Id = Guid.NewGuid().ToString(),
                    KontokorrentId = kontokorrentId,
                    Wert = bezahlung.Wert,
                    Zeitpunkt = bezahlung.Zeitpunkt ?? DateTime.UtcNow,
                    GeloeschteBezahlungId = null,
                    BearbeiteteBezahlungId = null,
                    LaufendeNummer = await NeueLaufNummer(kontokorrentId)
                };
                kontokorrentContext.Bezahlung.Add(b);
                await kontokorrentContext.SaveChangesAsync();
                return new ApiModels.v2.Bezahlung()
                {
                    BearbeitetBezahlungId = b.BearbeiteteBezahlungId,
                    Beschreibung = b.Beschreibung,
                    BezahlendePersonId = b.BezahlendePersonId,
                    EmpfaengerIds = bezahlung.Empfaenger,
                    GeloeschteBezahlungId = null,
                    Id = b.Id,
                    LaufendeNummer = b.LaufendeNummer,
                    Wert = b.Wert,
                    Zeitpunkt = new DateTimeOffset(b.Zeitpunkt, TimeSpan.Zero)
                };
            }
            finally
            {
                sem.Release();
            }
        }

        private async Task<int> NeueLaufNummer(string kontokorrentId)
        {
            var k = await kontokorrentContext.Kontokorrent.Where(v => v.Id == kontokorrentId).SingleAsync();
            k.LaufendeNummer++;
            return k.LaufendeNummer;
        }

        public async Task<ApiModels.v2.Bezahlung> DeleteAsync(string kontokorrentId, string id)
        {
            var b = await kontokorrentContext.Bezahlung.Where(p => p.Id == id && p.KontokorrentId == kontokorrentId).SingleOrDefaultAsync();
            if (null == b)
            {
                return null;
            }
            var sem = locks.GetOrAdd(b.KontokorrentId, new SemaphoreSlim(1));
            try
            {
                await sem.WaitAsync();
                var loeschung = new Bezahlung()
                {
                    Beschreibung = $"Löschen von ${b.Beschreibung}",
                    BezahlendePersonId = b.BezahlendePersonId,
                    Id = Guid.NewGuid().ToString(),
                    KontokorrentId = b.KontokorrentId,
                    Wert = 0,
                    Zeitpunkt = DateTime.UtcNow,
                    GeloeschteBezahlungId = b.Id,
                    BearbeiteteBezahlungId = null,
                    LaufendeNummer = await NeueLaufNummer(b.KontokorrentId)
                };
                kontokorrentContext.Bezahlung.Add(loeschung);
                await kontokorrentContext.SaveChangesAsync();
                return new ApiModels.v2.Bezahlung()
                {
                    BearbeitetBezahlungId = loeschung.BearbeiteteBezahlungId,
                    Beschreibung = loeschung.Beschreibung,
                    BezahlendePersonId = loeschung.BezahlendePersonId,
                    EmpfaengerIds = new string[0],
                    GeloeschteBezahlungId = loeschung.GeloeschteBezahlungId,
                    Id = b.Id,
                    LaufendeNummer = b.LaufendeNummer,
                    Wert = b.Wert,
                    Zeitpunkt = new DateTimeOffset(loeschung.Zeitpunkt, TimeSpan.Zero)
                };

            }
            finally
            {
                sem.Release();
            }
        }

        public async Task<ApiModels.v2.Bezahlung> EditAsync(string id, GeaenderteBezahlung request, string kontokorrentId)
        {
            var payment = kontokorrentContext.Bezahlung.Where(v => v.Id == id && v.KontokorrentId == kontokorrentId)
                .Include(v => v.Emfpaenger)
                .Include(v => v.BearbeitendeBezahlungen)
                .Include(v => v.LoeschendeBezahlungen)
                .SingleOrDefault();
            if (null == payment)
            {
                throw new BezahlungEditException(BezahlungEditException.NichtGefunden);
            }
            if (payment.BearbeitendeBezahlungen.Count > 0 ||
                payment.LoeschendeBezahlungen.Count > 0)
            {
                throw new BezahlungEditException(BezahlungEditException.NichtEditierbar);
            }
            var b = new Bezahlung()
            {
                Beschreibung = request.Beschreibung,
                BezahlendePersonId = payment.BezahlendePersonId,
                Emfpaenger = request.Empfaenger.Select(p => new EmfpaengerInBezahlung()
                {
                    EmpfaengerId = p
                }).ToList(),
                Id = Guid.NewGuid().ToString(),
                KontokorrentId = kontokorrentId,
                Wert = request.Wert,
                Zeitpunkt = request.Zeitpunkt,
                GeloeschteBezahlungId = null,
                BearbeiteteBezahlungId = payment.Id,
                LaufendeNummer = await NeueLaufNummer(kontokorrentId)
            };
            await kontokorrentContext.SaveChangesAsync();
            return new ApiModels.v2.Bezahlung()
            {
                BearbeitetBezahlungId = b.BearbeiteteBezahlungId,
                Beschreibung = b.Beschreibung,
                BezahlendePersonId = b.BezahlendePersonId,
                EmpfaengerIds = request.Empfaenger,
                GeloeschteBezahlungId = null,
                Id = b.Id,
                LaufendeNummer = b.LaufendeNummer,
                Wert = b.Wert,
                Zeitpunkt = new DateTimeOffset(b.Zeitpunkt, TimeSpan.Zero)
            };
        }

        public async Task<Models.Bezahlung[]> GueltigeAuflisten(string kontokorrentId)
        {
            return await kontokorrentContext.Bezahlung
                .Where(p => p.KontokorrentId == kontokorrentId &&
                 p.BearbeitendeBezahlungen.Count == 0 &&
                 p.LoeschendeBezahlungen.Count == 0)
                .OrderByDescending(v => v.Zeitpunkt)
                .Select(BezahlungMapper.ToModel).ToArrayAsync();
        }

        public async Task<ApiModels.v2.Bezahlung[]> AlleAuflistenAb(string kontokorrentId, int? ab = null)
        {
            return await kontokorrentContext.Bezahlung
                .Where(p => p.KontokorrentId == kontokorrentId &&
                 p.BearbeitendeBezahlungen.Count == 0 &&
                 p.LoeschendeBezahlungen.Count == 0 &&
                 p.LaufendeNummer > ab)
                .OrderByDescending(v => v.LaufendeNummer)
                .Select(BezahlungMapper.ToModelApiV2Model).ToArrayAsync();
        }
    }
}
