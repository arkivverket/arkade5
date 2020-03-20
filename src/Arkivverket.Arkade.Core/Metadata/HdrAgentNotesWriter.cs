using System.Collections.Generic;
using System.Linq;

namespace Arkivverket.Arkade.Core.Metadata
{
    public class HdrAgentNotesWriter
    {
        private readonly List<string> _notes = new List<string>();
        private readonly List<string> _fieldNames = new List<string>();

        private const string MetaNoteTemplate = "notescontent:{0}";

        private const string Address = "Address";
        private const string Telephone = "Telephone";
        private const string Email = "Email";

        private const string Version = "Version";
        private const string Type = "Type";
        private const string TypeVersion = "TypeVersion";

        public void AddAddress(string address)
        {
            _notes.Add(address);

            _fieldNames.Add(Address);
        }

        public void AddTelephone(string telephone)
        {
            _notes.Add(telephone);

            _fieldNames.Add(Telephone);
        }

        public void AddEmail(string email)
        {
            _notes.Add(email);

            _fieldNames.Add(Email);
        }

        public void AddVersion(string version)
        {
            _notes.Add(version);

            _fieldNames.Add(Version);
        }

        public void AddType(string type)
        {
            _notes.Add(type);

            _fieldNames.Add(Type);
        }

        public void AddTypeVersion(string typeVersion)
        {
            _notes.Add(typeVersion);

            _fieldNames.Add(TypeVersion);
        }

        public bool HasNotes()
        {
            return _notes.Any();
        }

        public string[] GetNotes()
        {
            if (_fieldNames.Any())
            {
                _notes.Add(string.Format(MetaNoteTemplate, string.Join(",", _fieldNames)));
            }

            return _notes.ToArray();
        }
    }
}
