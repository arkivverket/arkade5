using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public abstract class DataType
    {
        private readonly List<string> _nullValues;

        protected DataType()
        {
        }

        public abstract bool IsValid(string s);

        protected DataType(List<string> nullValues)
        {
            _nullValues = nullValues;

        }

        public bool IsNull(string s)
        {
            return _nullValues != null && _nullValues.Contains(s);
        }
    }
}