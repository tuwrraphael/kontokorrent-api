using Kontokorrent.Models;
using System.Threading.Tasks;

namespace Kontokorrent.Services
{
    public interface IPersonService
    {
        Task<Person> PersonHinzufuegen(string name, string kontokorrentId, BenutzerID? benutzer);
    }
}
