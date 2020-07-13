using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.EF
{
    public class KontokorrentRepository : IKontokorrentRepository
    {
        private const int MinimumPayments = 20;
        private const int MinimumHistoryDays = 3;

        private readonly KontokorrentContext kontokorrentContext;
        private readonly IPersonRepository personRepository;

        public KontokorrentRepository(KontokorrentContext kontokorrentContext, IPersonRepository personRepository)
        {
            this.kontokorrentContext = kontokorrentContext;
            this.personRepository = personRepository;
        }

        public async Task<KontokorrentErstellung> CreateAsync(NeuerKontokorrent kontokorrent)
        {
            var k = new Kontokorrent()
            {
                OeffentlicherName = kontokorrent.OeffentlicherName,
                Id = kontokorrent.Id,
                Name = kontokorrent.Name,
                Privat = kontokorrent.Privat
            };
            kontokorrentContext.Kontokorrent.Add(k);
            try
            {
                await kontokorrentContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                var inner = e.InnerException as SqliteException;
                if (null != inner && 19 == inner.SqliteErrorCode)
                {
                    throw new NameExistsException();
                }
            }
            PersonenStatus[] newPersons = new PersonenStatus[0];
            if (null != kontokorrent.Personen)
            {
                newPersons = await Task.WhenAll(kontokorrent.Personen.Select(async person => await personRepository.CreateAsync(person, k.Id))
                    .Select(async v => new PersonenStatus()
                    {
                        Person = await v,
                        Wert = 0
                    }));
            }
            return new KontokorrentErstellung()
            {
                PersonenStatus = newPersons
            };
        }

        public async Task<string> GetIdAsync(string secret)
        {
            var kontokurrent = await kontokorrentContext.Kontokorrent.Where(p => p.OeffentlicherName == secret).SingleOrDefaultAsync();
            if (null != kontokurrent)
            {
                return kontokurrent.Id;
            }
            else return null;
        }

        public async Task<KontokorrentStatus> Get(string id)
        {
            var bezahlungen = await kontokorrentContext.Bezahlung.Where(p => p.KontokorrentId == id && !p.Deleted).Include(p => p.Emfpaenger).ToArrayAsync();
            var personen = await kontokorrentContext.Kontokorrent.Where(p => p.Id == id).SelectMany(p => p.Personen).ToArrayAsync();
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
            var paymentsQueryable = bezahlungen.AsQueryable().OrderByDescending(v => v.Zeitpunkt);
            var recentPayments = paymentsQueryable.Where(v => v.Zeitpunkt >= DateTime.Now.AddDays(-MinimumHistoryDays));
            if (recentPayments.Count() <= MinimumPayments)
            {
                recentPayments = paymentsQueryable.Take(MinimumPayments);
            }
            return new KontokorrentStatus()
            {
                PersonenStatus = status.Select(v => new PersonenStatus()
                {
                    Wert = v.EinzelSaldos.Sum(s => s.Saldo),
                    Person = v.Person
                }).ToArray(),
                LetzteBezahlungen = recentPayments.Select(BezahlungMapper.ToModel).ToArray()
            };
        }

        public async Task<bool> Exists(string id, string oeffentlicherName)
        {
            if (null != oeffentlicherName)
            {
                return await kontokorrentContext.Kontokorrent.Where(p => p.Id == id || oeffentlicherName == p.OeffentlicherName).AnyAsync();
            }
            else
            {
                return await kontokorrentContext.Kontokorrent.Where(p => p.Id == id).AnyAsync();
            }
        }
    }
}
