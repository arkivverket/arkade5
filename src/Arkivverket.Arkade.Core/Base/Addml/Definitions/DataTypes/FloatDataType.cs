using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class FloatDataType : DataType
    {
        private readonly string _fieldFormat;

        private string _numberDecimalSeparator;
        private string _numberGroupSeparator;

        private static readonly string[] AcceptedDecimalSeparators = {".", ","};
        private static readonly string[] AcceptedThousandSeparators = {".", ",", " "};


        public FloatDataType()
        {
        }

        private static readonly string[] AcceptedDecimalFormats =
        {
            "nn,nn",
            "nn.nn"
        };
        private static readonly string[] AcceptedThousandFormats =
        {
            "n.nnn,nn",
            "n nnn,nn",
            "n,nnn.nn",
            "n nnn.nn",
        };

        private readonly List<string> _acceptedFieldFormats =
            new(AcceptedDecimalFormats.Concat(AcceptedThousandFormats));

        public FloatDataType(string fieldFormat, List<string> nullValues) : base(fieldFormat, nullValues)
        {
            _fieldFormat = fieldFormat;
        }

        protected override void VerifyFieldFormat(string fieldFormat)
        {
            if (fieldFormat == null)
                return;

            if (AcceptedThousandFormats.Contains(fieldFormat))
            {
                _numberGroupSeparator = fieldFormat[1].ToString();
                _numberDecimalSeparator = fieldFormat[5].ToString();
            }
            else if (AcceptedDecimalFormats.Contains(fieldFormat))
            {
                _numberDecimalSeparator = fieldFormat[2].ToString();
            }
            else
            {
                string message = string.Format(ExceptionMessages.InvalidFieldFormatMessage, fieldFormat, "float",
                    string.Join(", ", _acceptedFieldFormats));
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

        public override bool IsValid(string s, bool isNullable)
        {
            bool isValid;
            if (_fieldFormat == null)
            {
                isValid = TryParseDecimalForAllAllowedFormats(s);
            }
            else
            {
                var nfi = new NumberFormatInfo
                {
                    NumberDecimalSeparator = _numberDecimalSeparator,
                    NumberGroupSeparator = _numberGroupSeparator?? (_numberDecimalSeparator == "." ? "," : "."),
                };

                if (_numberGroupSeparator == null)
                {
                    isValid = decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, nfi, out _);
                }
                else
                {
                    nfi.NumberGroupSeparator = _numberGroupSeparator;
                    isValid = decimal.TryParse(s,
                        NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, nfi, out _);
                }
            }
            
            return isValid || base.IsValid(s, isNullable);
        }

        private bool TryParseDecimalForAllAllowedFormats(string s)
        {
            var nfi = new NumberFormatInfo();

            foreach (string decimalSeparator in AcceptedDecimalSeparators)
            {
                nfi.NumberDecimalSeparator = decimalSeparator;
                foreach (string thousandSeparator in AcceptedThousandSeparators.Where(ts => ts != decimalSeparator))
                {
                    nfi.NumberGroupSeparator = thousandSeparator;
                    if (decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, nfi, out _))
                        return true;

                    if (decimal.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, nfi, out _))
                    {
                        VerifyFieldFormat($"n{thousandSeparator}nnn{decimalSeparator}nn");
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
