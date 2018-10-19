
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes;
using System;
using System.Globalization;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions
{
    public class FloatDataType : DataType
    {
        private readonly string _fieldFormat;

        private const string FieldFormatDecimalSeparator = "nn,nn";
        private const string FieldFormatDecimalAndThousandSeparator = "n.nnn,nn";

        public FloatDataType()
        {
        }

        private readonly List<string> _acceptedFieldFormats = new List<string>
        {
            null,
            FieldFormatDecimalSeparator,
            FieldFormatDecimalAndThousandSeparator
        };

        public FloatDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            _fieldFormat = fieldFormat;

            VerifyFieldFormat(fieldFormat);
        }

        private void VerifyFieldFormat(string fieldFormat)
        {
            if (!_acceptedFieldFormats.Contains(fieldFormat))
            {
                string message = "Illegal field format '" + fieldFormat + "'. Accepted field formats are " + string.Join(", ", _acceptedFieldFormats);
                throw new ArgumentException(message);
            }
        }


        protected bool Equals(FloatDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FloatDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }

        public override bool IsValid(string s)
        {
            if (_fieldFormat == FieldFormatDecimalSeparator)
            {
                return IsValidDecimalSeparatorFormat(s);
            }
            else if (_fieldFormat == FieldFormatDecimalAndThousandSeparator)
            {
                return IsValidDecimalAndThousandSeparatorFormat(s);
            }
            else
            {
                // If no field format
                return IsValidDecimalAndThousandSeparatorFormat(s);
            }
        }

        private bool IsValidDecimalAndThousandSeparatorFormat(string s)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";
            nfi.NumberGroupSeparator = ".";

            Decimal res;
            return Decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, nfi, out res);
        }

        private bool IsValidDecimalSeparatorFormat(string s)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ",";
            nfi.NumberGroupSeparator = ".";

            Decimal res;
            return Decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, nfi, out res);
        }
    }
}
