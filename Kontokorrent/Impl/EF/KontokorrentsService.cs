using Kontokorrent.ApiModels.v2;
using Kontokorrent.Models;
using Kontokorrent.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Impl.EF
{
    public class KontokorrentsService : IKontokorrentsService
    {
        private readonly KontokorrentContext _kontokorrentContext;
        private readonly IKontokorrentRepository _kontokorrentRepository;

        public KontokorrentsService(KontokorrentContext kontokorrentContext, IKontokorrentRepository kontokorrentRepository)
        {
            _kontokorrentContext = kontokorrentContext;
            _kontokorrentRepository = kontokorrentRepository;
        }

        public async Task<KontokorrentListenEintrag[]> Auflisten(BenutzerID benutzerID)
        {
            return await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id)
                .Include(v => v.Kontokorrent)
                .Select(v => new KontokorrentListenEintrag()
                {
                    Id = v.Kontokorrent.Id,
                    Name = v.Kontokorrent.Name
                }).ToArrayAsync();
        }

        public async Task Entfernen(string kontokorrentId, BenutzerID benutzerID)
        {
            var eintraege = await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id && v.KontokorrentId == kontokorrentId)
                .ToArrayAsync();
            foreach (var eintrag in eintraege)
            {
                _kontokorrentContext.BenutzerKontokorrent.Remove(eintrag);
            }
            await _kontokorrentContext.SaveChangesAsync();
        }

        private async Task Hinzufuegen(BenutzerID benutzerID, string kontokorrentId)
        {
            await _kontokorrentContext.BenutzerKontokorrent.AddAsync(new BenutzerKontokorrent()
            {
                BenutzerId = benutzerID.Id,
                KontokorrentId = kontokorrentId
            });
            await _kontokorrentContext.SaveChangesAsync();
        }

        public async Task Erstellen(NeuerKontokorrentRequest request, BenutzerID ersteller)
        {
            bool privat = false;
            string oeffentlicherName;
            if (string.IsNullOrEmpty(request.OeffentlicherName))
            {
                privat = true;
                oeffentlicherName = Guid.NewGuid().ToString();
            }
            else
            {
                privat = false;
                oeffentlicherName = request.OeffentlicherName;
            }
            await _kontokorrentRepository.CreateAsync(new NeuerKontokorrent()
            {
                Id = request.Id,
                Name = request.Name,
                OeffentlicherName = oeffentlicherName,
                Privat = privat,
                Personen = request.Personen.Select(p => new Models.NeuePerson()
                {
                    Name = p.Name
                }).ToArray()
            });
            await _kontokorrentContext.SaveChangesAsync();
            await Hinzufuegen(ersteller, request.Id);
        }

        public async Task<KontokorrentListenEintrag[]> HinzufuegenPerCode(string einladungsCode, BenutzerID benutzerID)
        {
            var code = await _kontokorrentContext.EinladungsCode.Where(v => v.Id == einladungsCode).SingleOrDefaultAsync();
            if (null == code)
            {
                return null;
            }
            await Hinzufuegen(benutzerID, code.KontokorrentId);
            return await Auflisten(benutzerID);
        }

        public async Task<KontokorrentListenEintrag[]> HinzufuegenPerOeffentlicherName(string oeffentlicherName, BenutzerID benutzerID)
        {
            var kontokorrentId = await _kontokorrentContext.Kontokorrent.Where(v => v.OeffentlicherName == oeffentlicherName && !v.Privat)
                .Select(v => v.Id)
                .SingleOrDefaultAsync();
            if (null == kontokorrentId)
            {
                return null;
            }
            await Hinzufuegen(benutzerID, kontokorrentId);
            return await Auflisten(benutzerID);
        }

        public async Task<bool> HasAccess(BenutzerID benutzerID, string kontokorrentId)
        {
            return await _kontokorrentContext.BenutzerKontokorrent.Where(v => v.BenutzerId == benutzerID.Id && v.KontokorrentId == kontokorrentId).AnyAsync();
        }
    }
}
