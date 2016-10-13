using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml
{
    public class Record
    {
        public List<Field> Fields { private set; get; }

        public Record(List<Field> fields)
        {
            Fields = fields;
        }
    }
}