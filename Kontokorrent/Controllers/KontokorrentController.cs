using System.Linq;
using System.Security.Claims;
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

        public KontokorrentController(IKontokorrentRepository repository)
        {
            this.repository = repository;
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
                await repository.CreateAsync(request);
                return Ok();
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
