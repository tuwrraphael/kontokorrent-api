using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IKontokorrentsService
    {
        Task Erstellen(NeuerKontokorrent request, BenutzerID? ersteller);
        Task<PersonenStatus[]> GetPersonenStatus(string kontokorrentId);
        Task<KontokorrentInfo[]> Auflisten(BenutzerID benutzerID);
        Task<KontokorrentInfo[]> HinzufuegenPerOeffentlicherName(string einladungsCode, BenutzerID benutzerID);
        Task<KontokorrentInfo[]> HinzufuegenPerCode(string einladungsCode, BenutzerID benutzerID);
        Task Entfernen(string kontokorrentId, BenutzerID benutzerID);
        Task<bool> HasAccess(BenutzerID benutzerID, string kontokorrentId);
        Task<string> IdByOeffentlicherName(string oeffentlicherName);
    }
}
