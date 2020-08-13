using Kontokorrent.ApiModels.v2;
using Kontokorrent.Models;
using Kontokorrent.Services;
using System.Threading.Tasks;

namespace Kontokorrent.Impl
{
    public class BezahlungenService : IBezahlungenService
    {
        private readonly IKontokorrentsService kontokorrentsService;
        private readonly IBezahlungRepository bezahlungRepository;

        public BezahlungenService(IKontokorrentsService kontokorrentsService, IBezahlungRepository bezahlungRepository)
        {
            this.kontokorrentsService = kontokorrentsService;
            this.bezahlungRepository = bezahlungRepository;
        }

        public async Task<ApiModels.v2.Bezahlung[]> AlleAuflistenAb(BenutzerID benutzer, string kontokorrentId, int? ab = null)
        {
            if (!await kontokorrentsService.HasAccess(benutzer, kontokorrentId))
            {
                return null;
            }
            return await bezahlungRepository.AlleAuflistenAb(kontokorrentId, ab);
        }

        public async Task<ApiModels.v2.Bezahlung> Bearbeiten(BenutzerID benutzer, string kontokorrentId, string id, BezahlungBearbeitenRequest request)
        {
            if (!await kontokorrentsService.HasAccess(benutzer, kontokorrentId))
            {
                return null;
            }

            var bez = await bezahlungRepository.EditAsync(id, new GeaenderteBezahlung()
            {
                Beschreibung = request.Beschreibung,
                Empfaenger = request.EmpfaengerIds,
                Wert = request.Wert,
                Zeitpunkt = request.Zeitpunkt.UtcDateTime
            }, kontokorrentId);
            return bez;
        }

        public async Task<ApiModels.v2.Bezahlung> Hinzufuegen(BenutzerID benutzer, string kontokorrentId, NeueBezahlungRequest request)
        {
            if (!await kontokorrentsService.HasAccess(benutzer, kontokorrentId))
            {
                return null;
            }
            var bez = await bezahlungRepository.CreateAsync(new NeueBezahlung()
            {
                Beschreibung = request.Beschreibung,
                BezahlendePerson = request.BezahlendePersonId,
                Empfaenger = request.EmpfaengerIds,
                Wert = request.Wert,
                Zeitpunkt = request.Zeitpunkt.UtcDateTime
            }, kontokorrentId);
            return bez;
        }

        public async Task<ApiModels.v2.Bezahlung> Loeschen(BenutzerID benutzer, string kontokorrentId, string id)
        {
            if (!await kontokorrentsService.HasAccess(benutzer, kontokorrentId))
            {
                return null;
            }
            var bez = await bezahlungRepository.DeleteAsync(kontokorrentId, id);
            return bez;
        }
    }
}
