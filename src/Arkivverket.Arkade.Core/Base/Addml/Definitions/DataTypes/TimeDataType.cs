﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class TimeDataType : DataType
    {
        private readonly string _timeFormat;

        public TimeDataType(string timeFormat, List<string> nullValues = null) : base(timeFormat, nullValues)
        {
            _timeFormat = timeFormat;
        }

        public override bool IsValid(string timeString, bool isNullable)
        {
            string escapedLowerCaseFormat = EscapeColonsAndDashes(_timeFormat).ToLower();

            return TimeSpan.TryParseExact(timeString, escapedLowerCaseFormat, CultureInfo.InvariantCulture, out _)
                || base.IsValid(timeString, isNullable);
        }

        protected override void VerifyFieldFormat(string fieldFormat)
        {
            throw new NotImplementedException();
        }

        private static string EscapeColonsAndDashes(string someString)
        {
            return someString.Replace(":", "\\:").Replace("-", "\\-");
        }
    }
}
