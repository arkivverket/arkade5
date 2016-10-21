using System;
using System.Globalization;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class DateDataType : DataType
    {
        private readonly string _fieldFormat;
        private readonly string _dateTimeFormat;

        public DateDataType(string fieldFormat)
        {
            _fieldFormat = fieldFormat;
            _dateTimeFormat = ConvertToDateTimeFormat(_fieldFormat);
        }

        private string ConvertToDateTimeFormat(string fieldFormat)
        {
            // TODO: Do we have to convert ADDML data fieldFormat til .NET format?
            return fieldFormat;
        }

        public DateTime Parse(string dateTimeString)
        {
            return DateTime.ParseExact(dateTimeString, _dateTimeFormat, CultureInfo.InvariantCulture);
        }

        protected bool Equals(DateDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DateDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }
    }
}
