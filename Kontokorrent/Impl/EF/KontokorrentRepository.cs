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
        private readonly KontokorrentContext kontokorrentContext;

        public KontokorrentRepository(KontokorrentContext kontokorrentContext)
        {
            this.kontokorrentContext = kontokorrentContext;
        }

        public async Task CreateAsync(NeuerKontokorrent kontokorrent)
        {
            var k = new Kontokorrent()
            {
                Secret = kontokorrent.Secret,
                Id = Guid.NewGuid().ToString()
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
        }

        public async Task<PersonenStatus[]> Get(string id)
        {
            var bezahlungen = await kontokorrentContext.Bezahlung.Where(p => p.KontokorrentId == id).Include(p => p.Emfpaenger).ToArrayAsync();
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
            var status = personenStatus.Select(p => new PersonenStatus()
            {
                Person = p.Value.Person,
                EinzelSaldos = p.Value.EinzelSaldos.Where(v => v.Value.Saldo != 0).Select(e => e.Value).ToArray(),
            }).ToArray();
            foreach (var s in status)
            {
                if (s.EinzelSaldos.Length > 0)
                {
                    s.Wert = s.EinzelSaldos.Sum(v => v.Saldo);
                }
            }
            return status;
        }

        public async Task<string> GetIdAsync(string secret)
        {
            var kontokurrent = await kontokorrentContext.Kontokorrent.Where(p => p.Secret == secret).SingleOrDefaultAsync();
            if (null != kontokurrent)
            {
                return kontokurrent.Id;
            }
            else return null;
        }
    }
}
