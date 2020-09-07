using Kontokorrent.Impl.EFV2;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl
{
    public class KontokorrentsService : IKontokorrentsService
    {
        private readonly KontokorrentV2Context _kontokorrentContext;

        public KontokorrentsService(KontokorrentV2Context kontokorrentContext)
        {
            _kontokorrentContext = kontokorrentContext;
        }

        public async Task<Models.KontokorrentInfo[]> Auflisten(BenutzerID benutzerID)
        {
            return await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id)
                .Include(v => v.Kontokorrent)
                .ThenInclude(v => v.Personen)
                .Select(v => new KontokorrentInfo()
                {
                    Id = v.Kontokorrent.Id,
                    Name = v.Kontokorrent.Name,
                    Personen = v.Kontokorrent.Personen.Select(d => new Models.Person()
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToArray()
                }).ToArrayAsync();
        }

        public async Task Entfernen(string kontokorrentId, BenutzerID benutzerID)
        {
            var eintraege = await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id && v.KontokorrentId == kontokorrentId)
                .ToArrayAsync();
            foreach (var eintrag in eintraege)
            {
                _kontokorrentContext.BenutzerKontokorrent.Remove(eintrag);
            }
            await _kontokorrentContext.SaveChangesAsync();
        }

        private async Task Hinzufuegen(BenutzerID benutzerID, string kontokorrentId)
        {
            await _kontokorrentContext.BenutzerKontokorrent.AddAsync(new BenutzerKontokorrent()
            {
                BenutzerId = benutzerID.Id,
                KontokorrentId = kontokorrentId
            });
            await _kontokorrentContext.SaveChangesAsync();
        }

        public async Task Erstellen(NeuerKontokorrent request, BenutzerID? ersteller)
        {
            await CreateAsync(request);
            await _kontokorrentContext.SaveChangesAsync();
            if (ersteller.HasValue)
            {
                await Hinzufuegen(ersteller.Value, request.Id);
            }
        }

        private async Task CreateAsync(NeuerKontokorrent kontokorrent)
        {
            var k = new Kontokorrent.Impl.EFV2.Kontokorrent()
            {
                LaufendeNummer = 0,
                Name = kontokorrent.Name,
                OeffentlicherName = kontokorrent.OeffentlicherName,
                Privat = kontokorrent.Privat,
                Id = kontokorrent.Id,
                Personen = kontokorrent.Personen.Select(v => new EFV2.Person()
                {
                    Name = v.Name,
                    Id = v.Id ?? Guid.NewGuid().ToString()
                }).ToList()
            };
            _kontokorrentContext.Kontokorrent.Add(k);
            try
            {
                await _kontokorrentContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                var inner = e.InnerException as SqliteException;
                if (null != inner && 19 == inner.SqliteErrorCode)
                {
                    throw new NameExistsException();
                }
            }
        }

        public async Task<KontokorrentInfo[]> HinzufuegenPerCode(string einladungsCode, BenutzerID benutzerID)
        {
            var code = await _kontokorrentContext.EinladungsCode.Where(v => v.Id == einladungsCode).SingleOrDefaultAsync();
            if (null == code)
            {
                return null;
            }
            await Hinzufuegen(benutzerID, code.KontokorrentId);
            return await Auflisten(benutzerID);
        }

        public async Task<KontokorrentInfo[]> HinzufuegenPerOeffentlicherName(string oeffentlicherName, BenutzerID benutzerID)
        {
            var kontokorrentId = await _kontokorrentContext.Kontokorrent.Where(v => v.OeffentlicherName == oeffentlicherName && !v.Privat)
                .Select(v => v.Id)
                .SingleOrDefaultAsync();
            if (null == kontokorrentId)
            {
                return null;
            }
            await Hinzufuegen(benutzerID, kontokorrentId);
            return await Auflisten(benutzerID);
        }

        public async Task<bool> HasAccess(BenutzerID benutzerID, string kontokorrentId)
        {
            return await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id && v.KontokorrentId == kontokorrentId).AnyAsync();
        }


        public async Task<PersonenStatus[]> GetPersonenStatus(string kontokorrentId)
        {
            var bezahlungen = await _kontokorrentContext.Aktionen
                            .Include(v => v.Bezahlung)
                            .Include(v => v.Bezahlung.Emfpaenger)
                            .ThenInclude(v => v.Empfaenger)
                            .Where(v => v.KontokorrentId == kontokorrentId
                            && null != v.Bezahlung
                            && null == v.Bezahlung.BearbeitendeAktion
                            && null == v.Bezahlung.LoeschendeAktion)
                            .OrderByDescending(v => v.Bezahlung.Zeitpunkt)
                            .Select(v => v.Bezahlung)
                            .ToArrayAsync();
            var personen = await _kontokorrentContext.Kontokorrent.Where(p => p.Id == kontokorrentId).SelectMany(p => p.Personen).ToArrayAsync();
            var personenStatus = personen.ToDictionary(k => k.Id, p => new
            {
                Person = new Models.Person()
                {
                    Id = p.Id,
                    Name = p.Name
                },
                Wert = 0,
                EinzelSaldos = personen
                .ToDictionary(a => a.Id, v => new EinzelSaldo()
                {
                    Betrifft = new Models.Person()
                    {
                        Id = v.Id,
                        Name = v.Name
                    },
                    Saldo = 0
                })
            });
            foreach (var b in bezahlungen)
            {
                var splitted = b.Wert / b.Emfpaenger.Count();
                foreach (var receiver in b.Emfpaenger)
                {
                    personenStatus[receiver.EmpfaengerId].EinzelSaldos[b.BezahlendePersonId].Saldo += splitted;
                    personenStatus[b.BezahlendePersonId].EinzelSaldos[receiver.EmpfaengerId].Saldo -= splitted;
                }
            }
            var status = personenStatus.Select(p => new
            {
                p.Value.Person,
                EinzelSaldos = p.Value.EinzelSaldos.Where(v => v.Value.Saldo != 0).Select(e => e.Value).ToArray(),
            }).ToArray();
            return status.Select(v => new PersonenStatus()
            {
                Wert = v.EinzelSaldos.Sum(s => s.Saldo),
                Person = v.Person
            }).ToArray();
        }

        public async Task<string> IdByOeffentlicherName(string oeffentlicherName)
        {
            return await _kontokorrentContext.Kontokorrent.Where(v => !v.Privat && v.OeffentlicherName == oeffentlicherName).Select(v => v.Id).SingleOrDefaultAsync();
        }
    }
}
