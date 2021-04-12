using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Arkivverket.Arkade.Core.Base.Addml
{
    class DelimiterFileRecordEnumerator : IEnumerator<string>
    {

        private StreamReader _stream;
        private string _delimiter;
        private string _foundRecord = string.Empty;


        public DelimiterFileRecordEnumerator(StreamReader stream, string delimiter)
        {
            _stream = stream;
            _delimiter = delimiter;
        }

        string IEnumerator<string>.Current
        {
            get
            {
                return _foundRecord;
            }
        }

        public object Current
        {
            get
            {
                return _foundRecord;
            }
        }

        public bool MoveNext()
        {
            StringBuilder strBld = new StringBuilder();
            int readChar;
            bool search = true;
            bool returnVal = false;

            while (search)
            {
                readChar = _stream.Read();

                if (readChar == -1)
                {
                    if (strBld.Length > 0)
                    {
                        _foundRecord = strBld.ToString();
                        returnVal = true;
                        search = false;
                    }
                    else
                    {
                        _foundRecord = null;
                        returnVal = false;
                        search = false;
                    }
                }
                else
                {
                    strBld.Append(Convert.ToChar(readChar));
                    if (_CheckIfEndOfStringContainsDelimiter(strBld, _delimiter))
                    {
                        _foundRecord = _ReturnStringWithoutDelimAtEndOfStringbuilder(strBld, _delimiter);
                        returnVal = true;
                        search = false;
                    }
                }
            }
            return returnVal;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _stream.Dispose();
        }


        private bool _CheckIfEndOfStringContainsDelimiter(StringBuilder sb, string delim)
        {
            if (sb.Length < delim.Length)
            {
                return false;
            }
            else
            {
                string endOfSb = sb.ToString(sb.Length - delim.Length, delim.Length);
                return endOfSb.Equals(delim);
            }
        }


        private string _ReturnStringWithoutDelimAtEndOfStringbuilder(StringBuilder sb, string delim)
        {
            if (sb.Length < delim.Length)
            {
                return String.Empty;
            }
            else
            {
                return (sb.ToString(0, sb.Length - delim.Length));
            }
        }

    }
}
