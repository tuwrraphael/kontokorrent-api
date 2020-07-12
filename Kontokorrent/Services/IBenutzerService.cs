using Kontokorrent.ApiModels.v2;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IBenutzerService
    {
        Task<bool> Exists(string id);
        Task Create(NeuerBenutzerRequest request);
        Task<bool> Validate(TokenRequest request);
    }
}
