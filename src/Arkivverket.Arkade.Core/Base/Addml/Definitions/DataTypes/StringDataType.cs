using Arkivverket.Arkade.Core.Util;
using System;
using System.Collections.Generic;
using Arkivverket.Arkade.Core.Resources;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class StringDataType : DataType
    {
        public static readonly StringDataType Default = new StringDataType();

        private readonly string _fieldFormat;

        private const string StringDataTypeBirthNumber = "fnr";
        private const string StringDataTypeOrganizationNumber = "org";
        private const string StringDataTypeAccountNumber = "knr";

        private readonly HashSet<string> _acceptedFieldFormats = new HashSet<string>
        {
            null,
            StringDataTypeBirthNumber,
            StringDataTypeOrganizationNumber,
            StringDataTypeAccountNumber
        };

        public StringDataType()
        {
        }

        public StringDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            _fieldFormat = fieldFormat;

            VerifyFieldFormat(fieldFormat);
        }

        private void VerifyFieldFormat(string fieldFormat)
        {
            if (!_acceptedFieldFormats.Contains(fieldFormat))
            {
                string message = string.Format(ExceptionMessages.InvalidFieldFormatMessage, fieldFormat, "string",
                    string.Join(", ", _acceptedFieldFormats));
                throw new ArgumentException(message);
            }
        }

        public override bool IsValid(string s, bool isNullable)
        {
            return _fieldFormat switch
            {
                StringDataTypeAccountNumber => NorwegianAccountNumber.Verify(s), 
                StringDataTypeBirthNumber => NorwegianBirthNumber.Verify(s), 
                StringDataTypeOrganizationNumber => NorwegianOrganizationNumber.Verify(s),
                _ => !string.IsNullOrWhiteSpace(s)
            } || base.IsValid(s, isNullable);
        }

        protected bool Equals(StringDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StringDataType)obj);
        }

        public override int GetHashCode()
        {
            return (_fieldFormat != null ? _fieldFormat.GetHashCode() : 0);
        }
    }
}
