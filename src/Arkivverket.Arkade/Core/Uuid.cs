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

        public string GetValue()
        {
            return _uuid;
        }

    }
}
