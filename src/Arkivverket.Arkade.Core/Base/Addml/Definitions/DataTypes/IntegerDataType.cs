using Arkivverket.Arkade.Core.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class IntegerDataType : DataType
    {
        private readonly string _fieldFormat;

        private static readonly string[] FieldFormatThousandSeparators =
        {
            "n.nnn", 
            "n,nnn",
            "n nnn",
        };
        private const string FieldFormatExponent = "nnE+exp";
        private const string ExponentSymbol = "E+";

        private readonly List<string> _acceptedFieldFormats = new(FieldFormatThousandSeparators.Concat(new [] {FieldFormatExponent}));

        private readonly string _thousandSeparator;

        public IntegerDataType(string fieldFormat = null, List<string> nullValues = null) : base(fieldFormat, nullValues)
        {
            _fieldFormat = fieldFormat;
            _thousandSeparator = ParseThousandSeparator(fieldFormat);
        }

        protected override void VerifyFieldFormat(string fieldFormat)
        {
            if (fieldFormat == null)
                return;

            if (!_acceptedFieldFormats.Contains(fieldFormat))
            {
                string message = string.Format(ExceptionMessages.InvalidFieldFormatMessage, fieldFormat, "integer",
                    string.Join(", ", _acceptedFieldFormats));
                throw new ArgumentException(message);
            }
        }

        private string ParseThousandSeparator(string fieldFormat)
        {
            return fieldFormat != null && FieldFormatThousandSeparators.Contains(fieldFormat)
                ? fieldFormat[1].ToString()
                : null;
        }

        public string GetThousandSeparator()
        {
            return _thousandSeparator;
        }

        protected bool Equals(IntegerDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntegerDataType) obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }

        public override bool IsValid(string s, bool isNullable)
        {
            bool isValid;
            string stringWithoutWhitespace = s.Replace(" ", "");

            if (FieldFormatThousandSeparators.Contains(_fieldFormat))
            {
                isValid = IsValidThousandSeparatorFormat(stringWithoutWhitespace);
            }
            else if (_fieldFormat == FieldFormatExponent)
            {
                isValid = IsValidExponentFormat(stringWithoutWhitespace);
            }
            else
            {
                isValid = IsValidIntegerFormat(stringWithoutWhitespace);
            }

            return isValid || base.IsValid(s, isNullable);
        }

        private bool IsValidIntegerFormat(string s)
        {
            return BigInteger.TryParse(s, NumberStyles.Integer, null, out _);
        }

        private bool IsValidExponentFormat(string s)
        {
            return s.Contains(ExponentSymbol) && 
                   BigInteger.TryParse(s, NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, null, out _);
        }

        private bool IsValidThousandSeparatorFormat(string s)
        {
            return TryRemoveThousandSeparator(s, out string modified) &&
                   BigInteger.TryParse(modified, NumberStyles.Integer, null, out _);
        }

        private bool TryRemoveThousandSeparator(string s, out string modifiedString)
        {
            // Remove every fourth character, starting at the end of the string
            var sb = new StringBuilder();
            modifiedString = s;
            char[] reversed = s.Reverse().ToArray();
            for (var i = 0; i < reversed.Length; i++)
            {
                char c = reversed[i];
                if ((i != reversed.Length - 1) && ((i + 1) % 4 == 0))
                {
                    if (c.ToString() != _thousandSeparator && !int.TryParse(c.ToString(), out _))
                    {
                        return false;
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            modifiedString = new string(sb.ToString().Reverse().ToArray());
            return true;
        }
    }
}
