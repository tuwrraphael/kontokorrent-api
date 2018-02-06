using Kontokorrent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IBezahlungRepository
    {
        Task<Bezahlung[]> ListAsync(string kontokorrentId);

        Task<Bezahlung> CreateAsync(NeueBezahlung bezahlung, string kontokorrentId);

        Task<bool> DeleteAsync(string id);
    }
}
