using System;
using System.Collections.Generic;
using System.Globalization;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class DateDataType : DataType
    {
        private readonly string _dateTimeFormat;
        private readonly string _fieldFormat;

        public DateDataType(string fieldFormat) : this(fieldFormat, null)
        {
        }

        public DateDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            _fieldFormat = fieldFormat;
            _dateTimeFormat = ConvertToDateTimeFormat(_fieldFormat);
        }

        private string ConvertToDateTimeFormat(string fieldFormat)
        {
            return fieldFormat;
        }

        public DateTimeOffset Parse(string dateTimeString)
        {
            DateTimeOffset dto = DateTimeOffset.ParseExact
                       (dateTimeString, _dateTimeFormat, CultureInfo.InvariantCulture);
            return dto;
        }

        protected bool Equals(DateDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DateDataType) obj);
        }

        public override int GetHashCode()
        {
            return _fieldFormat?.GetHashCode() ?? 0;
        }

        public override bool IsValid(string s)
        {
            DateTimeOffset res;
            return DateTimeOffset.TryParseExact(s, _dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out res);
        }
    }
}