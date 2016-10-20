using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class IntegerDataType : FieldType
    {
        private string fieldFormat;

        public IntegerDataType(string fieldFormat)
        {
            this.fieldFormat = fieldFormat;
        }
    }
}
