using System;
using System.Collections.Generic;
using System.Globalization;

namespace Arkivverket.Arkade.Core.Addml.Definitions.DataTypes
{
    public class TimeDataType : DataType
    {
        private readonly string _timeFormat;

        public TimeDataType(string timeFormat, List<string> nullValues = null) : base(nullValues)
        {
            _timeFormat = timeFormat;
        }

        public override bool IsValid(string timeString)
        {
            TimeSpan unUsed;

            string escapedLowerCaseFormat = EscapeColonsAndDashes(_timeFormat).ToLower();

            return TimeSpan.TryParseExact(timeString, escapedLowerCaseFormat, CultureInfo.InvariantCulture, out unUsed);
        }

        private static string EscapeColonsAndDashes(string someString)
        {
            return someString.Replace(":", "\\:").Replace("-", "\\-");
        }
    }
}
