using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.EF
{
    public class PersonRepository : IPersonRepository
    {
        private readonly KontokorrentContext kontokorrentContext;

        public PersonRepository(KontokorrentContext kontokorrentContext)
        {
            this.kontokorrentContext = kontokorrentContext;
        }

        public async Task<Models.Person> CreateAsync(NeuePerson person, string kontokorrentId)
        {
            var p = new Person()
            {
                Name = person.Name,
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
                Name = person.Name,
                Id = p.Id
            };
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await kontokorrentContext.Person.Where(p => p.Id == id).AnyAsync();
        }
    }
}
