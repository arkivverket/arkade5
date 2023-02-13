using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public abstract class DataType
    {
        private readonly List<string> _nullValues;

        protected DataType()
        {
        }

        public virtual bool IsValid(string s, bool isNullable)
        {
            return IsValidNullValue(s) || isNullable && string.IsNullOrEmpty(s);
        }

        protected DataType(List<string> nullValues)
        {
            _nullValues = nullValues;
        }

        public bool IsValidNullValue(string s)
        {
            return _nullValues != null && _nullValues.Contains(s);
        }
    }
}