using System;
using System.Linq;
using System.Linq.Expressions;

namespace Kontokorrent.Impl.EF
{
    public static class BezahlungMapper
    {
        public static readonly Expression<Func<Bezahlung, Models.Bezahlung>> ToModel =
            r => new Models.Bezahlung()
            {
                Beschreibung = r.Beschreibung,
                BezahlendePerson = new Models.Person()
                {
                    Id = r.BezahlendePerson.Id,
                    Name = r.BezahlendePerson.Name
                },
                Id = r.Id,
                Empfaenger = r.Emfpaenger.Select(v => new Models.Person()
                {
                    Id = v.EmpfaengerId,
                    Name = v.Empfaenger.Name
                }).ToArray(),
                Wert = r.Wert,
                Zeitpunkt = r.Zeitpunkt
            };

        public static readonly Expression<Func<Bezahlung, ApiModels.v2.Bezahlung>> ToModelApiV2Model =
            r => new ApiModels.v2.Bezahlung()
            {
                Beschreibung = r.Beschreibung,
                BezahlendePersonId = r.BezahlendePersonId,
                Id = r.Id,
                EmpfaengerIds = r.Emfpaenger.Select(v => v.EmpfaengerId).ToArray(),
                Wert = r.Wert,
                Zeitpunkt = r.Zeitpunkt,
                BearbeitetBezahlungId = r.BearbeiteteBezahlungId,
                LaufendeNummer = r.LaufendeNummer,
                GeloeschteBezahlungId = r.GeloeschteBezahlungId
            };
    }
}
