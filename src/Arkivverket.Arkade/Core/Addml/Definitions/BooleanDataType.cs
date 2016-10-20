using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    class BooleanDataType : FieldType
    {
        private string fieldFormat;

        public BooleanDataType(string fieldFormat)
        {
            this.fieldFormat = fieldFormat;
        }
    }
}
