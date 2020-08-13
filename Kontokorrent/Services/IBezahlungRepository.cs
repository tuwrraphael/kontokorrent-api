using Kontokorrent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IBezahlungRepository
    {
        Task<Models.Bezahlung[]> GueltigeAuflisten(string kontokorrentId);

        Task<ApiModels.v2.Bezahlung> CreateAsync(NeueBezahlung bezahlung, string kontokorrentId);

        Task<ApiModels.v2.Bezahlung> DeleteAsync(string kontokorrentId, string id);
        Task<ApiModels.v2.Bezahlung> EditAsync(string id, GeaenderteBezahlung request, string kontokorrentId);
        Task<ApiModels.v2.Bezahlung[]> AlleAuflistenAb(string kontokorrentId, int? ab = null);
    }
}
