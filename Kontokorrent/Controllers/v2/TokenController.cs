using Kontokorrent.ApiModels.v2;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Kontokorrent.Controllers.v2
{
    [Route("api/v2/token")]
    public class TokenController : Controller
    {
        private readonly IBenutzerService _userService;
        private readonly ITokenService _tokenService;

        public TokenController(IBenutzerService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody]TokenRequest request)
        {
            if (await _userService.Validate(request))
            {
                var tokenResult = await _tokenService.CreateTokenAsync(request.Id, TimeSpan.FromHours(1));
                return Ok(tokenResult);
            }
            return Unauthorized();
        }
    }
}
