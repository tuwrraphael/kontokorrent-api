using System.Threading.Tasks;
using Kontokorrent.ApiModels;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers.v1
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonService personService;

        public PersonsController(IPersonService personService)
        {
            this.personService = personService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuePersonRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var person = await personService.PersonHinzufuegen(request.Name, User.GetId().Id, null);
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
