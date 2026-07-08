using System;

namespace Arkivverket.Arkade.Core.Base
{
    public class Uuid
    {

        private readonly Guid _uuid;

        private Uuid(Guid uuid)
        {
            _uuid = uuid;
        }

        public static bool TryParse(string uuidString, out Uuid uuid)
        {
            if (Guid.TryParse(uuidString, out Guid guid))
            {
                uuid = new Uuid(guid);
                return true;
            }

            uuid = null;
            return false;
        }

        // The DIAS identity as declared in a METS OBJID attribute — conventionally prefixed
        // ("UUID:xxx…"), but a bare value must also be accepted: Arkade itself will drop the
        // prefix on write-out, and must keep loading its own output.
        public static bool TryParseFromMetsObjid(string objid, out Uuid uuid)
        {
            uuid = null;

            if (objid == null)
                return false;

            const string conventionalPrefix = "UUID:";

            string uuidString = objid.Trim();

            if (uuidString.StartsWith(conventionalPrefix, StringComparison.OrdinalIgnoreCase))
                uuidString = uuidString[conventionalPrefix.Length..];

            return TryParse(uuidString, out uuid);
        }

        public static Uuid Random()
        {
            Guid guid = Guid.NewGuid();
            return new Uuid(guid);
        }

        public string GetValue()
        {
            return _uuid.ToString();
        }

        public override string ToString()
        {
            return _uuid.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Uuid o = obj as Uuid;
            if (o == null)
            {
                return false;
            }

            return _uuid.Equals(o._uuid);
        }

        public override int GetHashCode()
        {
            return _uuid.GetHashCode();
        }

    }
}
