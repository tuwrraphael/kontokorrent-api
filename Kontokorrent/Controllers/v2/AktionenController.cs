using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Authorize]
    [Route("api/v2/kontokorrents")]
    public class AktionenController : Controller
    {
        private readonly IAktionenService aktionenService;

        public AktionenController(IAktionenService aktionenService)
        {
            this.aktionenService = aktionenService;
        }

        private ApiModels.v2.Aktion MapAktion(Models.Aktion aktion)
        {
            return new ApiModels.v2.Aktion()
            {
                BearbeiteteBezahlungId = aktion.BearbeiteteBezahlungId,
                Bezahlung = aktion.Bezahlung != null ? new Bezahlung()
                {
                    Beschreibung = aktion.Bezahlung.Beschreibung,
                    BezahlendePersonId = aktion.Bezahlung.BezahlendePerson.Id,
                    Id = aktion.Bezahlung.Id,
                    EmpfaengerIds = aktion.Bezahlung.Empfaenger.Select(i => i.Id).ToArray(),
                    Wert = aktion.Bezahlung.Wert,
                    Zeitpunkt = aktion.Bezahlung.Zeitpunkt
                } : null,
                GeloeschteBezahlungId = aktion.GeloeschteBezahlungId,
                LaufendeNummer = aktion.LaufendeNummer
            };
        }

        [HttpGet("{kontokorrentId}/aktionen")]
        public async Task<IActionResult> Get(string kontokorrentId, int? ab = null)
        {
            var res = (await aktionenService.Auflisten(User.GetId(), kontokorrentId, ab));
            if (null == res)
            {
                return NotFound();
            }
            else
            {
                return Ok(res.Select(MapAktion).ToArray());
            }
        }

        [HttpPost("{kontokorrentId}/aktionen")]
        [Consumes("application/vnd+kontokorrent.loeschenaktion+json")]
        public async Task<IActionResult> BezahlungLoeschen(string kontokorrentId, [FromBody] BezahlungLoeschenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var deleted = await aktionenService.BezahlungLoeschen(User.GetId(), kontokorrentId, request.Id);
            if (null == deleted)
            {
                return NotFound();
            }
            return Ok(MapAktion(deleted));
        }

        [HttpPost("{kontokorrentId}/aktionen")]
        [Consumes("application/vnd+kontokorrent.hinzufuegenaktion+json")]
        public async Task<IActionResult> Create(string kontokorrentId, [FromBody] NeueBezahlungRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (request.EmpfaengerIds.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            try
            {
                var bezahlung = await aktionenService.BezahlungHinzufuegen(User.GetId(), kontokorrentId, new Models.NeueBezahlung()
                {
                    Id = request.Id,
                    Beschreibung = request.Beschreibung,
                    BezahlendePersonId = request.BezahlendePersonId,
                    EmpfaengerIds = request.EmpfaengerIds,
                    Wert = request.Wert,
                    Zeitpunkt = request.Zeitpunkt
                });
                if (null == bezahlung)
                {
                    return NotFound();
                }
                return Ok(MapAktion(bezahlung));
            }
            catch (PersonExistiertNichtException)
            {
                return NotFound("Person existiert nicht");
            }

        }

        [HttpPost("{kontokorrentId}/aktionen")]
        [Consumes("application/vnd+kontokorrent.bearbeitenaktion+json")]
        public async Task<IActionResult> Edit(string kontokorrentId, [FromBody] BezahlungBearbeitenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (request.EmpfaengerIds.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            try
            {
                var bezahlung = await aktionenService.BezahlungBearbeiten(User.GetId(), kontokorrentId, request.Id, new Models.GeaenderteBezahlung()
                {
                    Beschreibung = request.Beschreibung,
                    EmpfaengerIds = request.EmpfaengerIds,
                    Wert = request.Wert,
                    Zeitpunkt = request.Zeitpunkt,
                    BezahlendePersonId = request.BezahlendePersonId
                });
                if (null == bezahlung)
                {
                    return NotFound();
                }
                return Ok(MapAktion(bezahlung));
            }
            catch (BezahlungNichtEditierbarException)
            {
                return UnprocessableEntity();
            }
            catch (PersonExistiertNichtException)
            {
                return BadRequest("Person existiert nicht");
            }
        }
    }
}

