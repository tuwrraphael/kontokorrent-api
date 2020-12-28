using Kontokorrent.Models;
using System.Linq;
using System.Security.Claims;

namespace Kontokorrent.Controllers
{
    public static class ClaimsPrincipalExtension
    {
        public static BenutzerID GetId(this ClaimsPrincipal user)
        {
            var val = user.Claims.Where(p => p.Type == ClaimTypes.NameIdentifier).SingleOrDefault() ??
                user.Claims.Where(p => p.Type == ClaimTypes.Name).SingleOrDefault();
            return new BenutzerID(val.Value);
        }
    }
}
