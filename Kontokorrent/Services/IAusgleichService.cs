using Kontokorrent.Models;

namespace Kontokorrent.Services
{
    public interface IAusgleichService
    {
        KontokorrentAusgleich GetAusgleich(PersonenStatus[] personenStatus, Bezahlung[] bezahlungen, AusgleichRequest ausgleichRequest);
    }
}
