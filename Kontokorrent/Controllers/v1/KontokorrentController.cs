using System;
using System.Linq;
using System.Threading.Tasks;
using Kontokorrent.ApiModels.v1;
using Kontokorrent.Controllers;
using Kontokorrent.Services;
using Kontokorrent.Services.v1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllersv.v1
{
    [Route("api/[controller]")]
    public class KontokorrentController : Controller
    {
        private readonly IKontokorrentsService kontokorrentsService;
        private readonly ITokenService tokenService;
        private readonly IBezahlungenService bezahlungenService;

        public KontokorrentController(IKontokorrentsService kontokorrentsService, ITokenService tokenService,
            IBezahlungenService bezahlungenService)
        {
            this.kontokorrentsService = kontokorrentsService;
            this.tokenService = tokenService;
            this.bezahlungenService = bezahlungenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuerKontokorrentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var newId = Guid.NewGuid().ToString();
                await kontokorrentsService.Erstellen(new Models.NeuerKontokorrent()
                {
                    Id = newId,
                    Name = request.Secret,
                    OeffentlicherName = request.Secret.ToLower(),
                    Personen = request.Personen.Select(p => new Models.NeuePerson()
                    {
                        Name = p.Name
                    }).ToArray(),
                    Privat = false
                }, null);
                var personenStatus = await kontokorrentsService.GetPersonenStatus(newId);

                var response = new NeuerKontokorrentResponse()
                {
                    PersonenStatus = personenStatus.Select(p => new ApiModels.v1.PersonenStatus()
                    {
                        Person = new ApiModels.v1.Person()
                        {
                            Id = p.Person.Id,
                            Name = p.Person.Name
                        },
                        Wert = p.Wert
                    }).ToArray(),
                    Token = (await tokenService.CreateTokenAsync(newId)).Token
                };
                return Ok(response);
            }
            catch (NameExistsException)
            {
                return BadRequest("Secret existiert bereits");
            }
        }

        [Authorize]
        public async Task<IActionResult> Get()
        {
            var personenStatus = await kontokorrentsService.GetPersonenStatus(User.GetId().Id);

            var response = new KontokorrentResponse()
            {
                PersonenStatus = personenStatus.Select(p => new ApiModels.v1.PersonenStatus()
                {
                    Person = new Person()
                    {
                        Id = p.Person.Id,
                        Name = p.Person.Name
                    },
                    Wert = p.Wert
                }).ToArray(),
                LetzteBezahlungen = (await bezahlungenService.LetzteAuflisten(this.User.GetId().Id, 3, 20)).Select(v => new Bezahlung()
                {
                    Beschreibung = v.Beschreibung,
                    BezahlendePerson = new Person()
                    {
                        Id = v.BezahlendePerson.Id,
                        Name = v.BezahlendePerson.Name
                    },
                    Id = v.Id,
                    Empfaenger = v.Empfaenger.Select(d => new Person()
                    {
                        Id = d.Id,
                        Name = d.Name
                    }).ToArray(),
                    Wert = v.Wert,
                    Zeitpunkt = v.Zeitpunkt.UtcDateTime
                }).ToArray()
            };
            return Ok(response);
        }
    }
}
