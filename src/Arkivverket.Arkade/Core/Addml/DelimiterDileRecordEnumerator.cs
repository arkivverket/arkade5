using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    class DelimiterDileRecordEnumerator : IEnumerator
    {

        private StreamReader _stream;
        private string _delimiter;

        public DelimiterDileRecordEnumerator(StreamReader stream, string delimiter)
        {
            _stream = stream;
            _delimiter = delimiter;
        }

    public object Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
