using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IAktionenService
    {
        Task<Aktion> BezahlungHinzufuegen(BenutzerID? benutzer, string kontokorrentId, NeueBezahlung bezahlung);
        Task<Aktion> BezahlungLoeschen(BenutzerID? benutzer, string kontokorrentId, string id);
        Task<Aktion> BezahlungBearbeiten(BenutzerID? benutzer, string kontokorrentId, string id, GeaenderteBezahlung request);
        Task<Aktion[]> Auflisten(BenutzerID benutzer, string kontokorrentId, int? ab);
    }
}
