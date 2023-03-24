using System.Xml;

namespace Arkivverket.Arkade.Core.Util
{
    class XmlReaderLineInfoUtil
    {
        private readonly IXmlLineInfo _xmlLineInfo;
        private int _previousLine;
        private int _currentLinePosition;
        private readonly long _fileSize;

        private long _position;

        public XmlReaderLineInfoUtil(XmlReader xmlReader, long fileSize)
        {
            _xmlLineInfo = (IXmlLineInfo) xmlReader;
            _position = 0;
            _fileSize = fileSize;
        }

        public void UpdatePosition()
        {
            if (_xmlLineInfo.LineNumber != _previousLine)
            {
                _currentLinePosition = _xmlLineInfo.LinePosition;
                _position += _currentLinePosition;
                _previousLine = _xmlLineInfo.LineNumber;
            }
            else
            {
                _position += _xmlLineInfo.LinePosition - _currentLinePosition;
                _currentLinePosition = _xmlLineInfo.LinePosition;
            }
        }

        public int GetProgressPercentage()
        {
            return (int) (_position / _fileSize * 100);
        }

    }
}
