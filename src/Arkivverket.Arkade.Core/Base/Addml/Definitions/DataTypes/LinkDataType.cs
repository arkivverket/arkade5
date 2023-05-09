
using System.Collections.Generic;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class LinkDataType : DataType
    {
        private readonly string _fieldFormat;

        private const string LinkDataTypeForeignKey = "forkey";
        private const string LinkDataTypeDocumentReference = "doc";
        private const string LinkDataTypeWebsite = "www";
        private const string LinkDataTypeUrl = "url";


        public LinkDataType()
        {
        }

        public LinkDataType(string fieldFormat, List<string> nullValues) : base(nullValues)
        {
            _fieldFormat = fieldFormat.ToLower();
        }

        protected bool Equals(LinkDataType other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LinkDataType) obj);
        }

        public override int GetHashCode()
        {
            return 1;
        }

        public override bool IsValid(string s, bool isNullable)
        {
            return _fieldFormat switch
            {
                LinkDataTypeForeignKey => ValidateForeignKey(s),
                LinkDataTypeDocumentReference => ValidateDocumentReference(s),
                LinkDataTypeWebsite or LinkDataTypeUrl => ValidateUrl(s),
                _ => !string.IsNullOrWhiteSpace(s)
            } || base.IsValid(s, isNullable);
        }

        private bool ValidateForeignKey(string foreignKey)
        {
            return true;
        }

        private bool ValidateDocumentReference(string documentReference)
        {
            return true;
        }

        private static bool ValidateUrl(string documentReference)
        {
            return documentReference[0..7].ToLower().Equals("http://") || 
                   documentReference[0..8].ToLower().Equals("https://") ||
                   documentReference[0..4].ToLower().Equals("www.");
        }
    }
}
