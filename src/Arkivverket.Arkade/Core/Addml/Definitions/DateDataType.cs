using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class DateDataType : FieldType
    {
        private string fieldFormat;

        public DateDataType(string fieldFormat)
        {
            this.fieldFormat = fieldFormat;
        }
    }
}
