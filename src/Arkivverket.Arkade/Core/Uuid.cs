using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core
{
    public class Uuid
    {

        private string _uuid;

        private Uuid(string uuid)
        {
            _uuid = uuid;
        }

        public static Uuid Of(string uuid)
        {
            return new Uuid(uuid);
        }

        public static Uuid Random()
        {
            Guid guid = Guid.NewGuid();
            return new Uuid(guid.ToString());
        }

        public string GetValue()
        {
            return _uuid;
        }

        public override string ToString()
        {
            return _uuid;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Uuid o = obj as Uuid;
            if ((System.Object)o == null)
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
