using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kontokorrent.Controllers
{
    [Route("api/[controller]")]
    public class TokenController : Controller
    {
        private readonly IKontokorrentRepository repository;
        private readonly JWTOptions options;

        public TokenController(IKontokorrentRepository repository, IOptions<JWTOptions> optionsAccessor)
        {
            this.repository = repository;
            options = optionsAccessor.Value;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest request)
        {
            var id = await repository.GetIdAsync(request.Secret);
            if (null != id)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: options.IssuerAudience,
                    audience: options.IssuerAudience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(7),
                    signingCredentials: creds);
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            return Unauthorized();
        }
    }
}
