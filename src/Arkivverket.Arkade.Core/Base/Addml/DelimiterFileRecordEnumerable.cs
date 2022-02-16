using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    class DelimiterFileRecordEnumerable : IEnumerable<string>
    {

        private StreamReader _stream;
        private string _delimiter;
        private string _quotingChar;

        public DelimiterFileRecordEnumerable(StreamReader stream, string delimiter, string quotingChar)
        {
            _stream = stream;
            _delimiter = delimiter;
            _quotingChar = quotingChar;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new DelimiterFileRecordEnumerator(_stream, _delimiter, _quotingChar);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }

}
