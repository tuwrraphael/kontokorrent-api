using Kontokorrent.ApiModels.v2;
using Kontokorrent.Impl.EFV2;
using Kontokorrent.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Kontokorrent.Impl
{
    public class BenutzerService : IBenutzerService
    {
        private readonly KontokorrentV2Context _kontokorrentContext;
        private readonly IKontokorrentsService _kontokorrentsService;

        public BenutzerService(KontokorrentV2Context kontokorrentContext, IKontokorrentsService kontokorrentsService)
        {
            _kontokorrentContext = kontokorrentContext;
            _kontokorrentsService = kontokorrentsService;
        }

        public async Task Create(NeuerBenutzerRequest request)
        {
            if (!string.IsNullOrEmpty(request.OeffentlicherName))
            {
                if (!await _kontokorrentContext.Kontokorrent.AnyAsync(v => v.OeffentlicherName == request.OeffentlicherName))
                {
                    throw new KontokorrentNotFoundException();
                }
            }
            if (!string.IsNullOrEmpty(request.EinladungsCode))
            {
                if (!await _kontokorrentContext.EinladungsCode.AnyAsync(v => v.Id == request.EinladungsCode))
                {
                    throw new KontokorrentNotFoundException();
                }
            }
            string hashedSecret;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(request.Secret));
                hashedSecret = Convert.ToBase64String(bytes);
            }
            await _kontokorrentContext.BenutzerSecret.AddAsync(new BenutzerSecret()
            {
                BenutzerId = request.Id,
                HashedSecret = hashedSecret
            });
            await _kontokorrentContext.SaveChangesAsync();
            var benutzerId = new Models.BenutzerID(request.Id);
            if (!string.IsNullOrEmpty(request.OeffentlicherName))
            {
                await _kontokorrentsService.HinzufuegenPerOeffentlicherName(request.OeffentlicherName, benutzerId);
            }
            else if (!string.IsNullOrEmpty(request.EinladungsCode))
            {
                await _kontokorrentsService.HinzufuegenPerCode(request.EinladungsCode, benutzerId);
            }
        }

        public async Task<bool> Exists(string id)
        {
            return await _kontokorrentContext.BenutzerSecret.Where(d => d.BenutzerId == id).AnyAsync();
        }

        public async Task<bool> Validate(ApiModels.v2.TokenRequest request)
        {
            string hashedSecret;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(request.Secret));
                hashedSecret = Convert.ToBase64String(bytes);
            }
            return await _kontokorrentContext.BenutzerSecret.Where(d => d.BenutzerId == request.Id && d.HashedSecret == hashedSecret).AnyAsync();
        }
    }
}
