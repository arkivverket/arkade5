using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    class DelimiterFileRecordEnumerable : IEnumerable
    {

        private StreamReader _stream;
        private String _delimiter;

        public DelimiterFileRecordEnumerable(StreamReader stream, string delimiter)
        {
            _stream = stream;
            _delimiter = delimiter;
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new DelimiterDileRecordEnumerator(_stream, _delimiter);
        }
    }

}
