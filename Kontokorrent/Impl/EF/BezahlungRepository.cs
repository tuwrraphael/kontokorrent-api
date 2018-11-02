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

        private async Task<Models.Bezahlung> SelectAsync(string id)
        {
            var res = await kontokorrentContext.Bezahlung.Where(p => p.Id == id)
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
                Zeitpunkt = bezahlung.Zeitpunkt ?? DateTime.Now,
                Deleted = false
            };
            kontokorrentContext.Bezahlung.Add(b);
            await kontokorrentContext.SaveChangesAsync();
            return await SelectAsync(b.Id);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var b = await kontokorrentContext.Bezahlung.Where(p => p.Id == id).SingleOrDefaultAsync();
            if (null != b)
            {
                b.Deleted = true;
                await kontokorrentContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Models.Bezahlung> EditAsync(string id, GeaenderteBezahlung request, string kontokorrentId)
        {
            var payment = kontokorrentContext.Bezahlung.Where(v => v.Id == id && v.KontokorrentId == kontokorrentId)
                .Include(v => v.Emfpaenger).SingleOrDefault();
            if (null == payment)
            {
                return null;
            }
            payment.BearbeitetAm = DateTime.Now;
            payment.Beschreibung = request.Beschreibung;
            payment.Zeitpunkt = request.Zeitpunkt;
            payment.Wert = request.Wert;
            var delete = payment.Emfpaenger.Where(v => !request.Empfaenger.Contains(v.EmpfaengerId));
            var add = request.Empfaenger.Where(v => !payment.Emfpaenger.Any(x => x.EmpfaengerId == v));
            foreach (var empfaengerInBezahlung in delete)
            {
                kontokorrentContext.EmfpaengerInBezahlung.Remove(empfaengerInBezahlung);
            }
            foreach (var empfaengerId in add)
            {
                await kontokorrentContext.EmfpaengerInBezahlung.AddAsync(new EmfpaengerInBezahlung()
                {
                    BezahlungId = id,
                    EmpfaengerId = empfaengerId
                });
            }
            await kontokorrentContext.SaveChangesAsync();
            return await SelectAsync(id);
        }

        public async Task<Models.Bezahlung[]> ListAsync(string kontokorrentId)
        {
            return await kontokorrentContext.Bezahlung
                .Where(p => p.KontokorrentId == kontokorrentId && !p.Deleted)
                .OrderByDescending(v => v.Zeitpunkt)
                .Select(BezahlungMapper.ToModel).ToArrayAsync();
        }
    }
}
