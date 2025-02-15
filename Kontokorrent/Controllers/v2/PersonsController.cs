using System.Threading.Tasks;
using Kontokorrent.ApiModels;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers.v2
{
    [Authorize]
    [Route("api/v2/kontokorrents")]
    public class PersonsController : Controller
    {
        private readonly IPersonService personService;

        public PersonsController(IPersonService personService)
        {
            this.personService = personService;
        }

        [HttpPost("{kontokorrentId}/personen")]
        public async Task<IActionResult> Create(string kontokorrentId, [FromBody] NeuePersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var person = await personService.PersonHinzufuegen(request.Name, kontokorrentId, User.GetId());
                return Ok(new NeuePersonResponse()
                {
                    Id = person.Id,
                    Name = person.Name
                });
            }
            catch (NameExistsException)
            {
                return BadRequest("Name existiert bereits");
            }
        }
    }
}
