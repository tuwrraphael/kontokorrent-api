using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services.v1
{
    public interface IBezahlungenService
    {
        Task<Bezahlung[]> LetzteAuflisten(string kontokorrentId, int historyDays, int minimum);
        Task<Bezahlung[]> GueltigeAuflisten(string kontokorrentId);
    }
}
