using Kontokorrent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IPersonRepository
    {
        Task<Person> CreateAsync(NeuePerson person, string kontokorrentId);
        Task<bool> ExistsAsync(string id);
    }
}
