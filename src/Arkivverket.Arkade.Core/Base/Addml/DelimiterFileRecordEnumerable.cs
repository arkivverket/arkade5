using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    class DelimiterFileRecordEnumerable : IEnumerable<string>
    {

        private StreamReader _stream;
        private string _delimiter;

        public DelimiterFileRecordEnumerable(StreamReader stream, string delimiter)
        {
            _stream = stream;
            _delimiter = delimiter;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new DelimiterFileRecordEnumerator(_stream, _delimiter);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }

}
