using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Kontokorrent.Impl
{
    public class TokenService : ITokenService
    {
        private JWTOptions options;

        public TokenService(IOptions<JWTOptions> optionsAccessor)
        {
            options = optionsAccessor.Value;
        }


        public async Task<TokenResult> CreateTokenAsync(string kontokorrentId, TimeSpan? expires = null)
        {
            var claims = new[]
{
                    new Claim(ClaimTypes.NameIdentifier, kontokorrentId)
                };
            var expiresAbs = expires.HasValue ? DateTimeOffset.Now.Add(expires.Value) : DateTimeOffset.Now.AddDays(7);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: options.IssuerAudience,
                audience: options.IssuerAudience,
                claims: claims,
                expires: expiresAbs.DateTime,
                signingCredentials: creds);
            return new TokenResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expires = expiresAbs.ToUnixTimeMilliseconds()
            };
        }
    }
}
