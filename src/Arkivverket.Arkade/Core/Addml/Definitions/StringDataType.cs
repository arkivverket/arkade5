using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml.Definitions
{
    public class StringDataType : FieldType
    {
        private readonly string _fieldFormat;

        public StringDataType()
        {
            
        }
        public StringDataType(string fieldFormat)
        {
            _fieldFormat = fieldFormat;
        }

    }
}
