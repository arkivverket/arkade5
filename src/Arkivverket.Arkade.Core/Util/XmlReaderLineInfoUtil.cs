using System.Xml;

namespace Arkivverket.Arkade.Core.Util
{
    class XmlReaderLineInfoUtil
    {
        private readonly IXmlLineInfo _xmlLineInfo;
        private int _previousLine;
        private int _currentLinePosition;
        private readonly long _fileSize;

        public int Position { get; private set; }

        public XmlReaderLineInfoUtil(XmlReader xmlReader, long fileSize)
        {
            _xmlLineInfo = (IXmlLineInfo) xmlReader;
            Position = 0;
            _fileSize = fileSize;
        }

        public void UpdatePosition()
        {
            if (_xmlLineInfo.LineNumber != _previousLine)
            {
                _currentLinePosition = _xmlLineInfo.LinePosition;
                Position += _currentLinePosition;
                _previousLine = _xmlLineInfo.LineNumber;
            }
            else
            {
                Position += _xmlLineInfo.LinePosition - _currentLinePosition;
                _currentLinePosition = _xmlLineInfo.LinePosition;
            }
        }

        public int GetProgressPercentage()
        {
            return (int)((double) Position / _fileSize * 100);
        }

    }
}
