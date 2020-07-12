using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Authorize]
    [Route("api/v2/kontokorrents")]
    public class KontokorrentsController : Controller
    {
        private readonly IKontokorrentRepository repository;
        private readonly IKontokorrentsService _kontokorrentsService;

        public KontokorrentsController(IKontokorrentRepository repository, IKontokorrentsService kontokorrentsService)
        {
            this.repository = repository;
            _kontokorrentsService = kontokorrentsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuerKontokorrentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (await repository.Exists(request.Id, request.OeffentlicherName))
            {
                return UnprocessableEntity("Kontokorrent existiert bereits");
            }
            await _kontokorrentsService.Erstellen(request, User.GetId());
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return Ok(await _kontokorrentsService.Auflisten(User.GetId()));
        }

        [HttpPut]
        public async Task<IActionResult> Hinzufuegen(string oeffentlicherName, string einladungsCode)
        {
            if (string.IsNullOrEmpty(oeffentlicherName) == string.IsNullOrEmpty(einladungsCode))
            {
                return BadRequest("Entweder oeffentlicherName oder einladungsCode angeben.");
            }
            if (!string.IsNullOrEmpty(oeffentlicherName))
            {
                var res = await _kontokorrentsService.HinzufuegenPerOeffentlicherName(oeffentlicherName, User.GetId());
                if (null == res)
                {
                    return NotFound();
                }
                return Ok(res);
            }
            else
            {
                var res = await _kontokorrentsService.HinzufuegenPerCode(einladungsCode, User.GetId());
                if (null == res)
                {
                    return NotFound();
                }
                return Ok(res);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Entfernen(string kontokorrentId)
        {
            if (string.IsNullOrEmpty(kontokorrentId))
            {
                return BadRequest();
            }
            await _kontokorrentsService.Entfernen(kontokorrentId, User.GetId());
            return Ok();
        }
    }
}

