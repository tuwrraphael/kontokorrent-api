using System.Threading.Tasks;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IKontokorrentRepository repository;
        private readonly ITokenService tokenService;

        public TokenController(IKontokorrentRepository repository, ITokenService tokenService)
        {
            this.repository = repository;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest request)
        {
            var id = await repository.GetIdAsync(request.Secret);
            if (null != id)
            {
                var tokenResult = await tokenService.CreateTokenAsync(id);
                return Ok(tokenResult);
            }
            return Unauthorized();
        }
    }
}
