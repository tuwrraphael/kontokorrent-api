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
    }
}
