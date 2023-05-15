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

        public static Uuid Of(string uuidString)
        {
            if (!Guid.TryParse(uuidString, out Guid uuid))
                throw new ArkadeException(string.Format(ExceptionMessages.InvalidUUID, uuidString));

            return new Uuid(uuid);
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
