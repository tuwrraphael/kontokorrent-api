using System;
using System.Linq;
using System.Threading.Tasks;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Kontokorrent.Services.v1;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers.v1
{
    [Route("api/[controller]")]
    public class PaymentsController : Controller
    {
        private readonly IBezahlungenService bezahlungenService;
        private readonly IAktionenService aktionenService;

        public PaymentsController(IBezahlungenService bezahlungenService, IAktionenService aktionenService)
        {
            this.bezahlungenService = bezahlungenService;
            this.aktionenService = aktionenService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = (await bezahlungenService.GueltigeAuflisten(this.User.GetId().Id)).Select(v => new ApiModels.v1.Bezahlung()
            {
                Beschreibung = v.Beschreibung,
                BezahlendePerson = new ApiModels.v1.Person()
                {
                    Id = v.BezahlendePerson.Id,
                    Name = v.BezahlendePerson.Name
                },
                Id = v.Id,
                Empfaenger = v.Empfaenger.Select(d => new ApiModels.v1.Person()
                {
                    Id = d.Id,
                    Name = d.Name
                }).ToArray(),
                Wert = v.Wert,
                Zeitpunkt = v.Zeitpunkt.UtcDateTime
            });
            return Ok(res);
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
            var deleted = await aktionenService.BezahlungLoeschen(null, User.GetId().Id, id);
            if (null == deleted)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApiModels.v1.NeueBezahlungRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (request.Empfaenger.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            try
            {
                var aktion = await aktionenService.BezahlungHinzufuegen(null, User.GetId().Id, new NeueBezahlung()
                {
                    Beschreibung = request.Beschreibung,
                    BezahlendePersonId = request.BezahlendePerson,
                    EmpfaengerIds = request.Empfaenger,
                    Wert = request.Wert,
                    Zeitpunkt = request.Zeitpunkt.HasValue ? new DateTimeOffset(request.Zeitpunkt.Value, TimeSpan.Zero) : DateTimeOffset.Now
                });
                var bezahlung = aktion.Bezahlung;
                return Ok(new ApiModels.v1.Bezahlung()
                {
                    Beschreibung = bezahlung.Beschreibung,
                    BezahlendePerson = new ApiModels.v1.Person()
                    {
                        Id = bezahlung.BezahlendePerson.Id,
                        Name = bezahlung.BezahlendePerson.Name
                    },
                    Id = bezahlung.Id,
                    Empfaenger = bezahlung.Empfaenger.Select(d => new ApiModels.v1.Person()
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToArray(),
                    Wert = bezahlung.Wert,
                    Zeitpunkt = bezahlung.Zeitpunkt.UtcDateTime
                });
            }
            catch (PersonExistiertNichtException)
            {
                return NotFound("Person existiert nicht");
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(string id, [FromBody] ApiModels.v1.BezahlungBearbeitenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (request.Empfaenger.Length == 0)
            {
                return BadRequest("Keine Emfpaenger angegeben");
            }
            try
            {
                var bezahlung = await aktionenService.BezahlungBearbeiten(null, User.GetId().Id, id, new GeaenderteBezahlung()
                {
                    Beschreibung = request.Beschreibung,
                    EmpfaengerIds = request.Empfaenger,
                    Wert = request.Wert,
                    Zeitpunkt = new DateTimeOffset(request.Zeitpunkt, TimeSpan.Zero),
                    BezahlendePersonId = null
                });
                if (null == bezahlung)
                {
                    return NotFound();
                }
                return Ok();
            }
            catch (PersonExistiertNichtException)
            {
                return NotFound("Person existiert nicht");
            }
            catch (BezahlungNichtEditierbarException)
            {
                return UnprocessableEntity();
            }

        }
    }
}
