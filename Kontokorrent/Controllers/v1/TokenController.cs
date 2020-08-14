using System.Threading.Tasks;
using Kontokorrent.ApiModels.v1;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kontokorrent.Controllers.v1
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IKontokorrentsService repository;
        private readonly ITokenService tokenService;

        public TokenController(IKontokorrentsService repository, ITokenService tokenService)
        {
            this.repository = repository;
            this.tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest request)
        {
            var id = await repository.IdByOeffentlicherName(request.Secret);
            if (null != id)
            {
                var tokenResult = await tokenService.CreateTokenAsync(id);
                return Ok(tokenResult);
            }
            return Unauthorized();
        }
    }
}
