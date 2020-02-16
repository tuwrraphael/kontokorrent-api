using System.Diagnostics;

namespace Kontokorrent.Models
{
    [DebuggerDisplay("{Name}")]
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}