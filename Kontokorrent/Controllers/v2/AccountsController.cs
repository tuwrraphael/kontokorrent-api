using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Route("api/v2/accounts")]
    public class AccountsController : Controller
    {
        private readonly IBenutzerService _userService;

        public AccountsController(IBenutzerService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Neu([FromBody] NeuerBenutzerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!string.IsNullOrEmpty(request.EinladungsCode) && !string.IsNullOrEmpty(request.OeffentlicherName))
            {
                return BadRequest("Entweder EinladungsCode oder OeffentlicherName angeben");
            }
            if (await _userService.Exists(request.Id))
            {
                return BadRequest("Benutzer existiert");
            }
            try
            {
                await _userService.Create(request);
            }
            catch (KontokorrentNotFoundException)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}