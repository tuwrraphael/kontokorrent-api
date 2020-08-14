using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Authorize]
    [Route("api/v2/kontokorrents")]
    public class KontokorrentsController : Controller
    {
        private readonly IKontokorrentsService _kontokorrentsService;

        public KontokorrentsController(IKontokorrentsService kontokorrentsService)
        {
            _kontokorrentsService = kontokorrentsService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuerKontokorrentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                await _kontokorrentsService.Erstellen(new Models.NeuerKontokorrent()
                {
                    Id = request.Id,
                    Name = request.Name,
                    OeffentlicherName = request.OeffentlicherName,
                    Personen = request.Personen.Select(v => new Models.NeuePerson() { Name = v.Name }).ToArray(),
                    Privat = !string.IsNullOrEmpty(request.OeffentlicherName)
                }, User.GetId());
            }
            catch (NameExistsException)
            {
                return UnprocessableEntity("Kontokorrent existiert bereits");
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            return base.Ok((await _kontokorrentsService.Auflisten(User.GetId())).Select(MapKontokorrentInfo));
        }

        private static ApiModels.v2.KontokorrentInfo MapKontokorrentInfo(Models.KontokorrentInfo v)
        {
            return new ApiModels.v2.KontokorrentInfo()
            {
                Id = v.Id,
                Name = v.Name,
                Personen = v.Personen.Select(d => new ApiModels.v2.Person() { Name = v.Name, Id = v.Id }).ToArray()
            };
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
                return Ok(res.Select(MapKontokorrentInfo));
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

