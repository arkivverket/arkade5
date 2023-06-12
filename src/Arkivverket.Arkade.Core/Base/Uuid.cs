using System;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base
{
    public class Uuid
    {

        private readonly Guid _uuid;

        private Uuid(Guid uuid)
        {
            _uuid = uuid; // NB! UUID-transfer
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
