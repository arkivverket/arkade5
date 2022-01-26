using System;

namespace Arkivverket.Arkade.Core.Base
{

    [Serializable]
    public class ArkadeAddmlDelimiterException : Exception
    {
        public string RecordName => (string)Data["RecordName"];
        public string RecordData => (string)Data["RecordData"];
        public string RecordNumber => (string)Data["RecordNumber"]!;


        public ArkadeAddmlDelimiterException(string message) : base(message)
        {
        }

        public ArkadeAddmlDelimiterException(string message, Exception inner) : base(message, inner)
        {
        }

        public ArkadeAddmlDelimiterException(string message, string recordName="", string recordData="", string recordNumber="") : base(message)
        {
            Data.Add("RecordName", recordName);
            Data.Add("RecordData", recordData);
            Data.Add("RecordNumber", recordNumber);
        }
    }
}

