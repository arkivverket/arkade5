using System;
using System.Collections.Generic;
using System.Globalization;
using Arkivverket.Arkade.Core.Util;

namespace Arkivverket.Arkade.Core.Base.Addml.Definitions.DataTypes
{
    public class DateDataType : DataType
    {
        private const string _cSharpDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";
        private readonly string _fieldFormat;
        private readonly DateFormat _dateFormat;

        public DateDataType(string fieldFormat) : this(fieldFormat, null)
        {
        }

        public DateDataType(string fieldFormat, List<string> nullValues) : base(fieldFormat, nullValues)
        {
            _fieldFormat = fieldFormat;
            _dateFormat = ParseCSharpDateTimeFormat(_fieldFormat);
        }

        private static DateFormat ParseCSharpDateTimeFormat(string fieldFormat)
        {
            if (Iso8601Format.CalendarDateFormats.Contains(fieldFormat))
                return DateFormat.Iso8601Calendar;

            if (Iso8601Format.OrdinalDateFormats.Contains(fieldFormat))
                return DateFormat.Iso8601Ordinal;
            
            if (Iso8601Format.WeekDateFormats.Contains(fieldFormat))
                return DateFormat.Iso8601Week;

            return DateFormat.CSharpDateTime;
        }

        public DateTimeOffset Parse(string dateTimeString)
        {
            return _dateFormat switch
            {
                DateFormat.CSharpDateTime => DateTimeOffset.ParseExact(dateTimeString, _fieldFormat, CultureInfo.InvariantCulture),
                DateFormat.Iso8601Calendar => DateTimeOffset.Parse(ConvertIso8601CalendarDateTimeStringToCSharpDateTimeString(dateTimeString)),
                DateFormat.Iso8601Ordinal => DateTimeOffset.Parse(ConvertIso8601OrdinalDateTimeStringToCSharpDateTimeString(dateTimeString)),
                DateFormat.Iso8601Week => DateTimeOffset.Parse(ConvertIso8601WeekDateTimeStringToCSharpDateTimeString(dateTimeString)),
                _ => throw new NotImplementedException($"Date format {_dateFormat} is not yet handled.")
            };
        }

        protected bool Equals(DateDataType other)
        {
            return string.Equals(_fieldFormat, other._fieldFormat);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DateDataType) obj);
        }

        public override int GetHashCode()
        {
            return _fieldFormat?.GetHashCode() ?? 0;
        }

        public override bool IsValid(string s, bool isNullable)
        {
            bool tryParseExact = _dateFormat switch
            {
                DateFormat.CSharpDateTime => DateTimeOffset.TryParseExact
                (
                    s, _fieldFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _
                ),
                DateFormat.Iso8601Calendar => DateTimeOffset.TryParseExact
                (
                    ConvertIso8601CalendarDateTimeStringToCSharpDateTimeString(s), _cSharpDateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _
                ),
                DateFormat.Iso8601Ordinal => DateTimeOffset.TryParseExact
                (
                    ConvertIso8601OrdinalDateTimeStringToCSharpDateTimeString(s), _cSharpDateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _
                ),
                DateFormat.Iso8601Week => DateTimeOffset.TryParseExact
                (
                    ConvertIso8601WeekDateTimeStringToCSharpDateTimeString(s), _cSharpDateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out _
                ),
                _ => false
            };
            bool baseIsValid = base.IsValid(s, isNullable);
            return tryParseExact || baseIsValid;
        }

        protected override void VerifyFieldFormat(string fieldFormat)
        {
            throw new NotImplementedException();
        }

        private static string ConvertIso8601CalendarDateTimeStringToCSharpDateTimeString(string iso8601DateTimeString)
        {
            string iso8601CalendarDateBasicRepresentation = Iso8601Format.ConvertToBasicRepresentation(iso8601DateTimeString);

            if (iso8601CalendarDateBasicRepresentation.Length < 8)
                return iso8601DateTimeString;

            if (!(int.TryParse(iso8601CalendarDateBasicRepresentation[..4], out int year) 
                  && int.TryParse(iso8601CalendarDateBasicRepresentation[4..6], out int month) 
                  && int.TryParse(iso8601CalendarDateBasicRepresentation[6..8], out int day)))
                return iso8601DateTimeString;

            var dateString = $"{year}-{month}-{day}";
            if (!DateTimeOffset.TryParse(dateString, out _))
                return iso8601DateTimeString;

            if (iso8601CalendarDateBasicRepresentation.Length == 8)
                return new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.FromHours(0)).ToString("O");

            if (TryConvertIso8601TimeRepresentationToCSharpFormat(iso8601CalendarDateBasicRepresentation[9..],
                    out (TimeSpan timeOfDay, TimeSpan timeZone) timeComponents))
            {
                if (DateTimeOffset.TryParse($"{dateString}T{timeComponents.timeOfDay}Z", out _))
                    return new DateTimeOffset(year, month, day,
                        timeComponents.timeOfDay.Hours, timeComponents.timeOfDay.Minutes, 
                        timeComponents.timeOfDay.Seconds, timeComponents.timeZone).ToString("O");
            }

            return iso8601DateTimeString;
        }

        private static string ConvertIso8601OrdinalDateTimeStringToCSharpDateTimeString(string iso8601DateTimeString)
        {
            string iso8601OrdinalDateBasicRepresentation = Iso8601Format.ConvertToBasicRepresentation(iso8601DateTimeString);

            if (iso8601OrdinalDateBasicRepresentation.Length < 7)
                return iso8601DateTimeString;

            if (!(int.TryParse(iso8601OrdinalDateBasicRepresentation[..4], out int year)
                  && int.TryParse((iso8601OrdinalDateBasicRepresentation[4..7]), out int dayNumber)))
                return iso8601DateTimeString;

            var monthNumber = 1;
            int daysInMonth = DateTime.DaysInMonth(year, monthNumber);

            while (dayNumber > daysInMonth)
            {
                if (monthNumber == 12)
                    return iso8601DateTimeString;

                dayNumber -= daysInMonth;
                monthNumber++;
                daysInMonth = DateTime.DaysInMonth(year, monthNumber);
            }

            if (iso8601OrdinalDateBasicRepresentation.Length == 7)
                return new DateTimeOffset(year, monthNumber, dayNumber, 0, 0, 0, TimeSpan.FromHours(0)).ToString("O");
            
            if (TryConvertIso8601TimeRepresentationToCSharpFormat(iso8601OrdinalDateBasicRepresentation[8..],
                    out (TimeSpan timeOfDay, TimeSpan timeZone) timeComponents))
                return new DateTimeOffset(year, monthNumber, dayNumber,
                    timeComponents.timeOfDay.Hours, timeComponents.timeOfDay.Minutes, timeComponents.timeOfDay.Seconds,
                    timeComponents.timeZone).ToString("O");
            
            return iso8601DateTimeString;
        }

        private static string ConvertIso8601WeekDateTimeStringToCSharpDateTimeString(string iso8601DateTimeString)
        {
            string iso8601WeekDateBasicRepresentation = Iso8601Format.ConvertToBasicRepresentation(iso8601DateTimeString);

            if (iso8601WeekDateBasicRepresentation.Length < 8)
                return iso8601DateTimeString;

            if (!(int.TryParse(iso8601WeekDateBasicRepresentation[..4], out int year)
                  && int.TryParse(iso8601WeekDateBasicRepresentation[5..7], out int weekOfYear)
                  && int.TryParse(iso8601WeekDateBasicRepresentation[7].ToString(), out int dayNumber)) ||
                weekOfYear > 53 || dayNumber is < 1 or > 7)
                return iso8601DateTimeString;

            // https://stackoverflow.com/questions/662379/calculate-date-from-week-number
            var jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            Calendar cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            int weekNumber = weekOfYear;
            // As we're adding days to a date in Week 1, we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
                weekNumber -= 1;

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            DateTime result = firstThursday.AddDays(weekNumber * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            result = result.AddDays(-3);

            // dayNumber[1-7], 1=monday, 7=sunday: https://no.wikipedia.org/wiki/ISO_8601 - Ukedatoer
            result = result.AddDays(dayNumber - 1);

            if (iso8601WeekDateBasicRepresentation.Length == 8)
                return new DateTimeOffset(result.Year, result.Month, result.Day, 0, 0, 0, TimeSpan.FromHours(0)).ToString("O");
            
            if (TryConvertIso8601TimeRepresentationToCSharpFormat(iso8601WeekDateBasicRepresentation[9..],
                    out (TimeSpan timeOfDay, TimeSpan timeZone) timeComponents))
                return new DateTimeOffset(result.Year, result.Month, result.Day,
                    timeComponents.timeOfDay.Hours, timeComponents.timeOfDay.Minutes, timeComponents.timeOfDay.Seconds,
                    timeComponents.timeZone).ToString("O");
            
            return iso8601DateTimeString;
        }

        private static bool TryConvertIso8601TimeRepresentationToCSharpFormat(string timeString, out (TimeSpan timeOfDay, TimeSpan timeZone) timeComponents)
        {
            timeComponents = default;

            var second = 0;
            var timeZoneHour = 0;
            var timeZoneMinute = 0;

            int length = timeString.Length;
            try
            {
                switch (length)
                {
                    case 11:
                        timeZoneMinute = int.Parse(timeString[9..11]);
                        goto case 9;
                    case 9:
                        timeZoneHour = int.Parse(timeString[6..9]);
                        goto case 6;
                    case 7:
                        if (timeString[6] != 'Z')
                            return false;
                        goto case 6;
                    case 6:
                        second = int.Parse(timeString[4..6]);
                        goto case 4;
                    case 4:
                        int minute = int.Parse(timeString[2..4]);
                        int hour = int.Parse(timeString[..2]);
                        timeComponents = (new TimeSpan(hour, minute, second), new TimeSpan(timeZoneHour, timeZoneMinute, 0));
                        return true;
                    default:
                        return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private enum DateFormat
        {
            Iso8601Calendar,
            Iso8601Week,
            Iso8601Ordinal,
            CSharpDateTime,
        }
    }
}