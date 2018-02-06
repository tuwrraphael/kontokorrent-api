using System.Threading.Tasks;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers
{
    [Route("api/[controller]")]
    public class PersonsController : Controller
    {
        private readonly IPersonRepository repository;

        public PersonsController(IPersonRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NeuePerson request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            try
            {
                var person = await repository.CreateAsync(request, User.GetKontokorrentId());
                return Ok(person);
            }
            catch(NameExistsException)
            {
                return BadRequest("Name existiert bereits");
            }           
        }
    }
}
