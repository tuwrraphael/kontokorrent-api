using Kontokorrent.ApiModels.v2;
using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IKontokorrentsService
    {
        Task Erstellen(NeuerKontokorrentRequest request, BenutzerID ersteller);
        Task<KontokorrentListenEintrag[]> Auflisten(BenutzerID benutzerID);
        Task<KontokorrentListenEintrag[]> HinzufuegenPerOeffentlicherName(string einladungsCode, BenutzerID benutzerID);
        Task<KontokorrentListenEintrag[]> HinzufuegenPerCode(string einladungsCode, BenutzerID benutzerID);
        Task Entfernen(string kontokorrentId, BenutzerID benutzerID);
    }
}
