using Kontokorrent.Models;
using System.Linq;
using System.Security.Claims;

namespace Kontokorrent.Controllers
{
    public static class ClaimsPrincipalExtension
    {
        public static BenutzerID GetId(this ClaimsPrincipal user)
        {
            return new BenutzerID(user.Claims.Where(p => p.Type == ClaimTypes.NameIdentifier).SingleOrDefault().Value);
        }
    }
}
