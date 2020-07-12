using Kontokorrent.Models;
using System;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface ITokenService
    {
        Task<TokenResult> CreateTokenAsync(string subject, TimeSpan? expires = null);
    }
}
