using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface ITokenService    
    {
        Task<TokenResult> CreateTokenAsync(string kontokorrentId);
    }
}
