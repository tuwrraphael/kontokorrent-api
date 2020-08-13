using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Route("api/v2/kontokorrents")]
    public class BezahlungenController : Controller
    {
        private readonly IBezahlungenService bezahlungenService;
        private readonly IPersonRepository personRepository;

        public BezahlungenController(IBezahlungenService bezahlungenService,
            IPersonRepository personRepository)
        {
            this.bezahlungenService = bezahlungenService;
            this.personRepository = personRepository;
        }

        [HttpGet("{kontokorrentId}/bezahlungen")]
        public async Task<IActionResult> Get(string kontokorrentId, int? ab = null)
        {
            var res = await bezahlungenService.AlleAuflistenAb(User.GetId(), kontokorrentId, ab);
            if (null == res)
            {
                return NotFound();
            }
            return Ok(res);
        }

        [HttpDelete("{kontokorrentId}/bezahlungen/{id}")]
        public async Task<IActionResult> DeleteRoute(string kontokorrentId, string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var deleted = await bezahlungenService.Loeschen(User.GetId(), kontokorrentId, id);
            if (null == deleted)
            {
                return NotFound();
            }
            return Ok(deleted);
        }

        [HttpPost("{kontokorrentId}/bezahlungen")]
        public async Task<IActionResult> Create(string kontokorrentId, [FromBody] NeueBezahlungRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!await personRepository.ExistsAsync(request.BezahlendePersonId))
            {
                return NotFound("Bezahlende Person existiert nicht");
            }
            if (request.EmpfaengerIds.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            var existsChecks = request.EmpfaengerIds.Select(personRepository.ExistsAsync);
            var res = await Task.WhenAll(existsChecks);
            if (!res.All(p => p))
            {
                return NotFound("Empfaenger existiert nicht");
            }
            var bezahlung = await bezahlungenService.Hinzufuegen(User.GetId(), kontokorrentId, request);
            if (null == bezahlung)
            {
                return NotFound();
            }
            return Ok(bezahlung);
        }

        [HttpPost("{kontokorrentId}/bezahlungen/{id}")]
        public async Task<IActionResult> Edit(string id, string kontokorrentId, [FromBody] BezahlungBearbeitenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (request.EmpfaengerIds.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            var existsChecks = request.EmpfaengerIds.Select(personRepository.ExistsAsync);
            var res = await Task.WhenAll(existsChecks);
            if (!res.All(p => p))
            {
                return BadRequest("Empfaenger existiert nicht");
            }
            try
            {
                var bezahlung = await bezahlungenService.Bearbeiten(User.GetId(), kontokorrentId, id, request);
                if (null == bezahlung)
                {
                    return NotFound();
                }
                return Ok(bezahlung);
            }
            catch (BezahlungEditException e)
            {
                return UnprocessableEntity(new BezahlungBearbeitenResponse()
                {
                    Erfolg = false,
                    FehlerGrund = e.Grund
                });
            }

        }
    }
}

