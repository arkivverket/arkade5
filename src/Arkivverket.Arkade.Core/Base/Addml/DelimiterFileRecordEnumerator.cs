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
        private string _quotingChar;
        private string _foundRecord = string.Empty;


        public DelimiterFileRecordEnumerator(StreamReader stream, string delimiter, string quotingChar)
        {
            _stream = stream;
            _delimiter = delimiter;
            _quotingChar = quotingChar;
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
            var stringBuilder = new StringBuilder();
            int readChar;
            bool search = true;
            bool returnVal = false;
            int numberOfQuotingCharsFound = 0;
            int quotingCharIndex = 0;

            while (search)
            {
                readChar = _stream.Read();

                if (readChar == -1)
                {
                    if (stringBuilder.Length > 0)
                    {
                        _foundRecord = stringBuilder.ToString();
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
                    var value = Convert.ToChar(readChar);
                    stringBuilder.Append(value);

                    if (_quotingChar != null && value == _quotingChar[quotingCharIndex])
                    {
                        quotingCharIndex++;

                        if (quotingCharIndex == _quotingChar.Length)
                        {
                            numberOfQuotingCharsFound++;
                            quotingCharIndex = 0;
                        }

                        continue;
                    }

                    quotingCharIndex = 0;

                    if (numberOfQuotingCharsFound % 2 == 0 && _CheckIfEndOfStringContainsDelimiter(stringBuilder, _delimiter))
                    {
                        _foundRecord = _ReturnStringWithoutDelimAtEndOfStringbuilder(stringBuilder, _delimiter);
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
