using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkivverket.Arkade.Core.Addml
{
    class DelimiterDileRecordEnumerator : IEnumerator<string>
    {

        private StreamReader _stream;
        private string _delimiter;
        private string _foundRecord = null;


        public DelimiterDileRecordEnumerator(StreamReader stream, string delimiter)
        {
            _stream = stream;
            _delimiter = delimiter;
        }

        string IEnumerator<string>.Current
        {
            get
            {
                if (!string.IsNullOrEmpty(_foundRecord))
                {
                    return _foundRecord;
                }
                else
                {
                    throw new Exception("Current element null error");
                }
            }
        }

        public object Current
        {
            get
            {
                if (!string.IsNullOrEmpty(_foundRecord))
                {
                    return _foundRecord;
                }
                else
                {
                    throw new Exception("Current element null error");
                }
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
                return(_ReturnStringWithoutDelimAtEndOfStringbuilder(sb, delim).Equals(delim));
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
                return (sb.ToString(sb.Length - delim.Length, sb.Length));
            }
        }

    }
}
