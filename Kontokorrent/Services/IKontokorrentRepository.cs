using Kontokorrent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IKontokorrentRepository
    {
        Task<string> GetIdAsync(string secret);

        Task CreateAsync(NeuerKontokorrent kontokorrent);

        Task<PersonenStatus[]> Get(string id);
    }
}
