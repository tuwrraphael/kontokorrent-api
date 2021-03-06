﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers
{
    [Route("api/[controller]")]
    public class KontokorrentController : Controller
    {
        private readonly IKontokorrentRepository repository;
        private readonly ITokenService tokenService;
        private readonly IAusgleichService ausgleichService;
        private readonly IBezahlungRepository bezahlungRepository;

        public KontokorrentController(IKontokorrentRepository repository, ITokenService tokenService,
            IAusgleichService ausgleichService, IBezahlungRepository bezahlungRepository)
        {
            this.repository = repository;
            this.tokenService = tokenService;
            this.ausgleichService = ausgleichService;
            this.bezahlungRepository = bezahlungRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuerKontokorrent request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var id = await repository.GetIdAsync(request.Secret);
            if (null != id)
            {
                return BadRequest("Kontokorrent existiert bereits");
            }
            try
            {
                var newId = Guid.NewGuid().ToString();
                var creation = await repository.CreateAsync(newId, request);
                creation.Token = (await tokenService.CreateTokenAsync(newId)).Token;
                return Ok(creation);
            }
            catch (NameExistsException)
            {
                return BadRequest("Secret existiert bereits");
            }
        }

        [Authorize]
        public async Task<IActionResult> Get()
        {
            return Ok(await repository.Get(User.GetKontokorrentId()));
        }

        [Authorize]
        [HttpPost("ausgleich")]
        public async Task<IActionResult> GetAusgleich([FromBody]AusgleichRequest ausgleichRequest)
        {
            var id = User.GetKontokorrentId();
            var status = await repository.Get(id);
            if (null != ausgleichRequest)
            {
                var personen = (ausgleichRequest.MussZahlungen ?? new GeforderteZahlung[0])
                    .Union((ausgleichRequest.BevorzugteZahlungen ?? new GeforderteZahlung[0]))
                    .SelectMany(v => new[] { v.PersonA, v.PersonB });
                foreach (var p in personen)
                {
                    if (!status.PersonenStatus.Any(d => d.Person.Id == p))
                    {
                        return BadRequest($"Person {p} existiert nicht.");
                    }
                }
            }

            return Ok(ausgleichService.GetAusgleich(status.PersonenStatus, await bezahlungRepository.ListAsync(id), ausgleichRequest));
        }
    }
}
