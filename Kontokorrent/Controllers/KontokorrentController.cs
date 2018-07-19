using System;
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
        private readonly IPersonRepository personRepository;

        public KontokorrentController(IKontokorrentRepository repository, ITokenService tokenService)
        {
            this.repository = repository;
            this.tokenService = tokenService;
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
    }
}
