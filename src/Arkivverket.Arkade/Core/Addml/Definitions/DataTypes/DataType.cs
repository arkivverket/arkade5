using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public abstract class DataType
    {
        private readonly List<string> _nullValues;

        protected DataType()
        {
        }

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