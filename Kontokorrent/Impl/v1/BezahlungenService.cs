using Kontokorrent.Impl.EFV2;
using Kontokorrent.Services.v1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.v1
{
    public class BezahlungenService : IBezahlungenService
    {
        private readonly KontokorrentV2Context _kontokorrentV2Context;

        public BezahlungenService(KontokorrentV2Context kontokorrentV2Context)
        {
            _kontokorrentV2Context = kontokorrentV2Context;
        }

        public async Task<Models.Bezahlung[]> GueltigeAuflisten(string kontokorrentId)
        {
            return await PaymentsQueryable(kontokorrentId).ToArrayAsync();
        }

        private IQueryable<Models.Bezahlung> PaymentsQueryable(string kontokorrentId)
        {
            return _kontokorrentV2Context.Aktionen
                            .Include(v => v.Bezahlung)
                            .Include(v => v.Bezahlung.Emfpaenger)
                            .ThenInclude(v => v.Empfaenger)
                            .Where(v => v.KontokorrentId == kontokorrentId
                            && null != v.Bezahlung
                            && null == v.Bezahlung.BearbeitendeAktion
                            && null == v.Bezahlung.LoeschendeAktion)
                            .OrderByDescending(v => v.Bezahlung.Zeitpunkt)
                            .Select(v => new Models.Bezahlung()
                            {
                                Zeitpunkt = v.Bezahlung.Zeitpunkt,
                                Beschreibung = v.Bezahlung.Beschreibung,
                                BezahlendePerson = new Models.Person()
                                {
                                    Id = v.Bezahlung.BezahlendePerson.Id,
                                    Name = v.Bezahlung.BezahlendePerson.Name
                                },
                                Empfaenger = v.Bezahlung.Emfpaenger.Select(d => new Models.Person()
                                {
                                    Name = d.Empfaenger.Name,
                                    Id = d.Empfaenger.Id
                                }).ToArray(),
                                Id = v.Bezahlung.Id,
                                Wert = v.Bezahlung.Wert
                            });
        }

        public async Task<Models.Bezahlung[]> LetzteAuflisten(string kontokorrentId, int historyDays, int minimum)
        {
            var paymentsQueryable = PaymentsQueryable(kontokorrentId);
            var recentPayments = paymentsQueryable.Where(v => v.Zeitpunkt >= DateTimeOffset.UtcNow.AddDays(-historyDays));
            if (recentPayments.Count() <= minimum)
            {
                recentPayments = paymentsQueryable.Take(minimum);
            }
            return await recentPayments.ToArrayAsync();
        }
    }
}
