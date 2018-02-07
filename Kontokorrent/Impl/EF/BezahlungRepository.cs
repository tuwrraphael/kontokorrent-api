using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.EF
{
    public class BezahlungRepository : IBezahlungRepository
    {
        private readonly KontokorrentContext kontokorrentContext;

        public BezahlungRepository(KontokorrentContext kontokorrentContext)
        {
            this.kontokorrentContext = kontokorrentContext;
        }

        public async Task<Models.Bezahlung> CreateAsync(NeueBezahlung bezahlung, string kontokorrentId)
        {

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
                Zeitpunkt = DateTime.Now
            };
            kontokorrentContext.Bezahlung.Add(b);
            await kontokorrentContext.SaveChangesAsync();
            var res = await kontokorrentContext.Bezahlung.Where(p => p.Id == b.Id)
                .Select(r => 
             new Models.Bezahlung()
            {
                Beschreibung = r.Beschreibung,
                BezahlendePerson = new Models.Person()
                {
                    Id = r.BezahlendePerson.Id,
                    Name = r.BezahlendePerson.Name
                },
                Id = r.Id,
                Empfaenger = r.Emfpaenger.Select(v => new Models.Person()
                {
                    Id = v.EmpfaengerId,
                    Name = v.Empfaenger.Name
                }).ToArray(),
                Wert = r.Wert,
                Zeitpunkt = r.Zeitpunkt
            }).SingleAsync();
            return res;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var b = await kontokorrentContext.Bezahlung.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (null != b)
            {
                
                kontokorrentContext.Bezahlung.Remove(b);
            }
            else
            {
                return false;
            }
            await kontokorrentContext.SaveChangesAsync();
            return true;
        }

        public async Task<Models.Bezahlung[]> ListAsync(string kontokorrentId)
        {
            return await kontokorrentContext.Bezahlung
                .Where(p => p.KontokorrentId == kontokorrentId)
                .Select(r =>
            new Models.Bezahlung()
            {
                Beschreibung = r.Beschreibung,
                BezahlendePerson = new Models.Person()
                {
                    Id = r.BezahlendePerson.Id,
                    Name = r.BezahlendePerson.Name
                },
                Id = r.Id,
                Empfaenger = r.Emfpaenger.Select(v => new Models.Person()
                {
                    Id = v.EmpfaengerId,
                    Name = v.Empfaenger.Name
                }).ToArray(),
                Wert = r.Wert,
                Zeitpunkt = r.Zeitpunkt
            }).ToArrayAsync();
        }
    }
}
