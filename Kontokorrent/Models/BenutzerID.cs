namespace Kontokorrent.Models
{
    public struct BenutzerID
    {
        public BenutzerID(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}
