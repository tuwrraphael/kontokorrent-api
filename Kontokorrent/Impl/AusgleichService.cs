using Kontokorrent.Models;
using Kontokorrent.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Kontokorrent.Impl
{
    public class AusgleichService : IAusgleichService
    {
        private PersonenStatus[] Anwenden(PersonenStatus[] personenStatus, IEnumerable<Ausgleichszahlung> ausgleichszahlungen)
        {
            var cloned = personenStatus.Select(s =>
                new PersonenStatus()
                {
                    Person = s.Person,
                    Wert = s.Wert
                }).ToArray();
            foreach (var zahlung in ausgleichszahlungen)
            {
                var bezahlenderStatus = cloned.Single(v => v.Person.Id == zahlung.BezahlendePerson.Id);
                var empfaengerStatus = cloned.Single(v => v.Person.Id == zahlung.Empfaenger.Id);
                bezahlenderStatus.Wert -= zahlung.Wert;
                empfaengerStatus.Wert += zahlung.Wert;
            }
            return cloned;
        }

        private bool IstAufgeloest(PersonenStatus[] personenStatus)
        {
            return personenStatus.All(s => IsCloseTo(0, s.Wert, 0.05));
        }

        private bool IsCloseTo(double a, double b, double tolerance = double.Epsilon)
        {
            return Math.Abs(a - b) < tolerance;
        }

        private Ausgleichszahlung[] GleicheAufloesen(PersonenStatus[] personenStatus)
        {
            return personenStatus.Where(v => v.Wert > 0).Select(bezahlender =>
               {
                   var empfaenger = personenStatus.FirstOrDefault(e => bezahlender.Person.Id != e.Person.Id &&
                        IsCloseTo(0, e.Wert + bezahlender.Wert));
                   if (null != empfaenger)
                   {
                       return new Ausgleichszahlung()
                       {
                           BezahlendePerson = bezahlender.Person,
                           Empfaenger = empfaenger.Person,
                           Wert = bezahlender.Wert
                       };
                   }
                   return null;
               }).Where(z => null != z).ToArray();
        }

        private Ausgleichszahlung[] MussZahlungen(PersonenStatus[] personenStatus, GeforderteZahlung[] mussZahlungen)
        {
            return mussZahlungen.Select(z =>
             {
                 var bezahlender = personenStatus.Single(v => v.Person.Id == z.PersonA);
                 var empfaenger = personenStatus.Single(v => v.Person.Id == z.PersonB);

                 return new Ausgleichszahlung()
                 {
                     BezahlendePerson = bezahlender.Person,
                     Empfaenger = empfaenger.Person,
                     Wert = bezahlender.Wert
                 };
             }).ToArray();
        }

        private Ausgleichszahlung GleichheitErzeugen(PersonenStatus[] personenStatus, ScoreComparer scoreComparer)
        {
            return personenStatus.Where(b => b.Wert > 0).Select(bezahlender =>
            {
                var empfaengerKandidaten = personenStatus.Where(empf => -empf.Wert > bezahlender.Wert);
                foreach (var empfaengerKandidat in empfaengerKandidaten)
                {
                    var ausgleichMoeglich = personenStatus.Any(bezahlender2 => bezahlender.Person.Id != bezahlender2.Person.Id && bezahlender2.Wert > 0
                     && IsCloseTo(empfaengerKandidat.Wert + bezahlender.Wert, -bezahlender2.Wert));
                    if (ausgleichMoeglich)
                    {
                        return new Ausgleichszahlung()
                        {
                            BezahlendePerson = bezahlender.Person,
                            Empfaenger = empfaengerKandidat.Person,
                            Wert = bezahlender.Wert
                        };
                    }
                }
                return null;
            }).Where(z => null != z)
            .OrderByDescending(z => z, scoreComparer)
            .FirstOrDefault();
        }

        private Ausgleichszahlung MoeglicheZahlung(PersonenStatus[] personenStatus, ScoreComparer scoreComparer)
        {
            var einfacheZahlungKandidaten = personenStatus.Where(b => b.Wert > 0)
                .SelectMany(bezahlender =>
                {
                    return personenStatus.Where(empf => empf.Person.Id != bezahlender.Person.Id && -empf.Wert > bezahlender.Wert)
                    .Select(empfaenger => new Ausgleichszahlung()
                    {
                        BezahlendePerson = bezahlender.Person,
                        Empfaenger = empfaenger.Person,
                        Wert = bezahlender.Wert
                    });
                }).OrderByDescending(z => z, scoreComparer);
            var einfacheZahlung = einfacheZahlungKandidaten.FirstOrDefault();
            if (null != einfacheZahlung)
            {
                return einfacheZahlung;
            }
            var teilzahlung = personenStatus.Where(b => b.Wert > 0)
                .SelectMany(bezahlender =>
                {
                    return personenStatus.Where(empf => empf.Person.Id != bezahlender.Person.Id && empf.Wert < 0)
                    .Select(empfaenger =>
                     new Ausgleichszahlung()
                     {
                         BezahlendePerson = bezahlender.Person,
                         Empfaenger = empfaenger.Person,
                         Wert = -empfaenger.Wert
                     });
                })
                .OrderByDescending(z => z, scoreComparer)
                .FirstOrDefault();
            return teilzahlung;
        }
        public KontokorrentAusgleich GetAusgleich(PersonenStatus[] personenStatus, Bezahlung[] bezahlungen, AusgleichRequest ausgleichRequest)
        {
            if (null == ausgleichRequest)
            {
                ausgleichRequest = new AusgleichRequest();
            }
            if (null == ausgleichRequest.BevorzugteZahlungen)
            {
                ausgleichRequest.BevorzugteZahlungen = new GeforderteZahlung[0];
            }
            if (null == ausgleichRequest.MussZahlungen)
            {
                ausgleichRequest.MussZahlungen = new GeforderteZahlung[0];
            }
            var scoreComparer = new ScoreComparer(personenStatus, bezahlungen, ausgleichRequest.BevorzugteZahlungen);
            var angewendete = new List<Ausgleichszahlung>();
            void aktualisieren(Ausgleichszahlung[] ausgleichszahlungen)
            {
                personenStatus = Anwenden(personenStatus, ausgleichszahlungen);
                angewendete.AddRange(ausgleichszahlungen);
            };
            void einzelneAnwenden(Ausgleichszahlung ausgleichszahlung)
            {
                personenStatus = Anwenden(personenStatus, new[] { ausgleichszahlung });
                angewendete.Add(ausgleichszahlung);
            };
            aktualisieren(MussZahlungen(personenStatus, ausgleichRequest.MussZahlungen));
            for (var i = 0; i < 1000; i++)
            {
                if (IstAufgeloest(personenStatus))
                {
                    return new KontokorrentAusgleich()
                    {
                        Ausgleichszahlungen = angewendete.ToArray()
                    };
                }
                aktualisieren(GleicheAufloesen(personenStatus));
                if (!IstAufgeloest(personenStatus))
                {
                    var gleichheitsZahlung = GleichheitErzeugen(personenStatus, scoreComparer);
                    if (null != gleichheitsZahlung)
                    {
                        einzelneAnwenden(gleichheitsZahlung);
                    }
                    else
                    {
                        var moeglicheZahlung = MoeglicheZahlung(personenStatus, scoreComparer);
                        if (null != moeglicheZahlung)
                        {
                            einzelneAnwenden(moeglicheZahlung);
                        }
                    }
                }
            }
            throw new Exception("Auflösen nicht möglich.");
        }

        [DebuggerDisplay("{PersonA} {PersonB} {Value}")]
        private class Score
        {
            public bool Is(Person a, Person b)
            {
                return (PersonA.Id == a.Id && PersonB.Id == b.Id) || (PersonA.Id == b.Id && PersonB.Id == a.Id);
            }
            public Person PersonA { get; set; }
            public Person PersonB { get; set; }
            public double Value { get; set; }
        }

        private class ScoreComparer : IComparer<Ausgleichszahlung>
        {
            private readonly Score[] _scores;

            public ScoreComparer(PersonenStatus[] personenStatus, Bezahlung[] bezahlungen, GeforderteZahlung[] bevorzugteZahlungen)
            {
                _scores = GetScores(personenStatus, bezahlungen);
                var max = _scores.Select(v => v.Value).Max();
                max += 1.0;
                foreach (var bevorzugteZahlung in bevorzugteZahlungen)
                {
                    var personA = personenStatus.Single(p => p.Person.Id == bevorzugteZahlung.PersonA).Person;
                    var personB = personenStatus.Single(p => p.Person.Id == bevorzugteZahlung.PersonB).Person;
                    _scores.Single(v => v.Is(personA, personB)).Value = max;
                }
            }

            public int Compare([AllowNull] Ausgleichszahlung x, [AllowNull] Ausgleichszahlung y)
            {
                if (null == x || null == y)
                {
                    return -1;
                }
                var xScore = _scores.Single(v => v.Is(x.BezahlendePerson, x.Empfaenger)).Value;
                var yScore = _scores.Single(v => v.Is(y.BezahlendePerson, y.Empfaenger)).Value;
                if (Math.Abs(xScore - yScore) < double.Epsilon)
                {
                    return 0;
                }
                else if (xScore < yScore)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            private Score[] GetScores(PersonenStatus[] personenStatus, Bezahlung[] bezahlungen)
            {
                var scores = new List<Score>();
                foreach (var pA in personenStatus)
                {
                    foreach (var pB in personenStatus)
                    {
                        if (pB.Person.Id != pA.Person.Id)
                        {
                            if (!scores.Any(s => s.Is(pA.Person, pB.Person)))
                            {
                                scores.Add(new Score()
                                {
                                    PersonA = pA.Person,
                                    PersonB = pB.Person,
                                    Value = 0
                                });
                            }
                        }
                    }
                }
                foreach (var b in bezahlungen)
                {
                    foreach (var e in b.Empfaenger)
                    {
                        if (b.BezahlendePerson.Id != e.Id)
                        {
                            var score = scores.Single(s => s.Is(b.BezahlendePerson, e));
                            score.Value += 1.0 / b.Empfaenger.Length;
                        }
                    }
                }
                return scores.ToArray();
            }
        }
    }
}
