using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Models
{
    public class KontokorrentInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string OeffentlicherName { get; set; }
        public Person[] Personen { get; set; }
    }
}
