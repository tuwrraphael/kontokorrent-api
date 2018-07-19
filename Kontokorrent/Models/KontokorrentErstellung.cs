using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent.Models
{
    public class KontokorrentErstellung
    {
        public PersonenStatus[] PersonenStatus { get; set; }
        public string Token { get; set; }
    }
}
