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
    public class PersonService : IPersonService
    {
        private readonly KontokorrentV2Context kontokorrentContext;

        public PersonService(KontokorrentV2Context kontokorrentContext)
        {
            this.kontokorrentContext = kontokorrentContext;
        }

        public async Task<Models.Person> PersonHinzufuegen(string name, string kontokorrentId, BenutzerID? benutzer)
        {
            var p = new EFV2.Person()
            {
                Name = name,
                KontokorrentId = kontokorrentId,
                Id = Guid.NewGuid().ToString()
            };

            kontokorrentContext.Person.Add(p);
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
            return new Models.Person()
            {
                Name = name,
                Id = p.Id
            };
        }
    }
}
