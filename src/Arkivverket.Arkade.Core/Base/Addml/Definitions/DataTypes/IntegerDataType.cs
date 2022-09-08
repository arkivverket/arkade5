
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

        public IntegerDataType()
        {
        }

        public IntegerDataType(string fieldFormat) : this(fieldFormat, null)
        {
        }

        public IntegerDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            VerifyFieldFormat(fieldFormat);

            _fieldFormat = fieldFormat;
            _thousandSeparator = ParseThousandSeparator(fieldFormat);
        }
        private void VerifyFieldFormat(string fieldFormat)
        {
            if (fieldFormat == null)
                return;

            if (!_acceptedFieldFormats.Contains(fieldFormat))
            {
                string message = "Illegal field format '" + fieldFormat + "' for data type 'integer'. Accepted field formats are " + string.Join(", ", _acceptedFieldFormats);
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
            } else if (_fieldFormat == FieldFormatExponent)
            {
                isValid = IsValidExponentFormat(stringWithoutWhitespace);
            } else
            {
                isValid = IsValidIntegerFormat(stringWithoutWhitespace);
            }

            return isValid || base.IsValid(s, isNullable);
        }

        private bool IsValidIntegerFormat(string s)
        {
            BigInteger res;
            return BigInteger.TryParse(s, NumberStyles.Integer, null, out res);
        }

        private bool IsValidExponentFormat(string s)
        {
            if (!s.Contains(ExponentSymbol))
            {
                return false;
            }

            BigInteger res;
            return BigInteger.TryParse(s, NumberStyles.AllowExponent | NumberStyles.AllowLeadingSign, null, out res);
        }

        private bool IsValidThousandSeparatorFormat(string s)
        {
            // Remove every fourth character, starting at the end of the string
            StringBuilder sb = new StringBuilder();
            char[] reversed = s.Reverse().ToArray();
            for (int i = 0; i < reversed.Length; i++)
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
            s = new string(sb.ToString().Reverse().ToArray());
            BigInteger res;
            return BigInteger.TryParse(s, NumberStyles.Integer, null, out res);
        }

    }
}
