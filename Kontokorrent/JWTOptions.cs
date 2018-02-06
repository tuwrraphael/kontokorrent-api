using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kontokorrent
{
    public class JWTOptions
    {
        public string IssuerAudience { get; set; }
        public string Secret { get; set; }
    }
}
