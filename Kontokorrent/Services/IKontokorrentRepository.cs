using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IKontokorrentRepository
    {
        Task<string> GetIdAsync(string secret);

        Task<KontokorrentErstellung> CreateAsync(string id, NeuerKontokorrent kontokorrent);

        Task<KontokorrentStatus> Get(string id);
    }
}
