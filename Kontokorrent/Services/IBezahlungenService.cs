using Kontokorrent.ApiModels.v2;
using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IBezahlungenService
    {
        Task<ApiModels.v2.Bezahlung> Hinzufuegen(BenutzerID benutzer, string kontokorrentId, NeueBezahlungRequest request);
        Task<ApiModels.v2.Bezahlung> Loeschen(BenutzerID benutzer, string kontokorrentId, string id);
        Task<ApiModels.v2.Bezahlung> Bearbeiten(BenutzerID benutzer, string kontokorrentId, string id, BezahlungBearbeitenRequest request);
        Task<ApiModels.v2.Bezahlung[]> AlleAuflistenAb(BenutzerID benutzer, string kontokorrentId, int? ab = null);
    }
}
