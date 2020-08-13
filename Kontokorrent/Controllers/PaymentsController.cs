using System.Linq;
using System.Threading.Tasks;
using Kontokorrent.ApiModels.v2;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers
{
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IBezahlungRepository repository;
        private readonly IPersonRepository personRepository;

        public PaymentsController(IBezahlungRepository repository,
            IPersonRepository personRepository)
        {
            this.repository = repository;
            this.personRepository = personRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await repository.GueltigeAuflisten(User.GetId().Id));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            return await DeleteRoute(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var deleted = await repository.DeleteAsync(User.GetId().Id, id);
            if (null == deleted)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeueBezahlung request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (!await personRepository.ExistsAsync(request.BezahlendePerson))
            {
                return NotFound("Bezahlende Person existiert nicht");
            }
            if (request.Empfaenger.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            var existsChecks = request.Empfaenger.Select(personRepository.ExistsAsync);
            var res = await Task.WhenAll(existsChecks);
            if (!res.All(p => p))
            {
                return NotFound("Empfaenger existiert nicht");
            }
            var bezahlung = await repository.CreateAsync(request, User.GetId().Id);
            return Ok(bezahlung);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] GeaenderteBezahlung request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (request.Empfaenger.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            var existsChecks = request.Empfaenger.Select(personRepository.ExistsAsync);
            var res = await Task.WhenAll(existsChecks);
            if (!res.All(p => p))
            {
                return BadRequest("Empfaenger existiert nicht");
            }
            try
            {
                var bezahlung = await repository.EditAsync(id, request, User.GetId().Id);
                if (null == bezahlung)
                {
                    return NotFound();
                }
                return Ok();
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
