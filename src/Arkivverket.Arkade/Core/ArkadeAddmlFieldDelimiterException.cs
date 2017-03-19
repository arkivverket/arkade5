using System;
using Arkivverket.Arkade.Core.Addml.Definitions;

namespace Arkivverket.Arkade.Core
{

    [Serializable]
    public class ArkadeAddmlFieldDelimiterException : Exception
    {
        public string RecordName => (string )this.Data["RecordName"];
        public string RecordData => (string)this.Data["RecordData"];


        public ArkadeAddmlFieldDelimiterException(string message) : base(message)
        {
        }

        public ArkadeAddmlFieldDelimiterException(string message, Exception inner) : base(message, inner)
        {
        }

        public ArkadeAddmlFieldDelimiterException(string message, string recordName, string recordData) : base(message)
        {
            this.Data.Add("RecordName", recordName);
            this.Data.Add("RecordData", recordData);
        }
    }
}

