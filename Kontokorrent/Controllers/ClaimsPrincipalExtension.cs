using System.Linq;
using System.Security.Claims;

namespace Kontokorrent.Controllers
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetKontokorrentId(this ClaimsPrincipal user)
        {
            return user.Claims.Where(p => p.Type == ClaimTypes.Name).SingleOrDefault().Value;
        }
    }
}
