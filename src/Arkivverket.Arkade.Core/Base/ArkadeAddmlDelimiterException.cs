using System;

namespace Arkivverket.Arkade.Core.Base
{

    [Serializable]
    public class ArkadeAddmlDelimiterException : Exception
    {
        public string RecordName => (string )this.Data["RecordName"];
        public string RecordData => (string)this.Data["RecordData"];


        public ArkadeAddmlDelimiterException(string message) : base(message)
        {
        }

        public ArkadeAddmlDelimiterException(string message, Exception inner) : base(message, inner)
        {
        }

        public ArkadeAddmlDelimiterException(string message, string recordName, string recordData) : base(message)
        {
            this.Data.Add("RecordName", recordName);
            this.Data.Add("RecordData", recordData);
        }
    }
}

