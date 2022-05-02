using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Arkivverket.Arkade.Core.Util
{
    internal static class Iso8601Format
    {
        private const string CalendarDateBasic = "YYYYMMDD";
        private const string CalendarDateExtended = "YYYY-MM-DD";
        private const string OrdinalDateBasic = "YYYYDDD";
        private const string OrdinalDateExtended = "YYYY-DDD";
        private const string WeekDateBasic = "YYYY\"W\"WWD";
        private const string WeekDateExtended= "YYYY-\"W\"WW-D";

        private const string TimeDesignator = "\"T\"";

        private const string TimeOfDayBasic = "hhmmss";
        private const string TimeOfDayExtended = "hh\":\"mm\":\"ss";

        private const string TimeZoneUtc = "\"Z\"";

        private const string TimeShiftSymbol = "±";

        private const string TimeZoneHourOffset = "hh";
        private const string TimeZoneOffsetBasic = "hhmm";
        private const string TimeZoneOffsetExtended = "hh\":\"mm";


        private static readonly string CalendarDateBasicComplete = string.Concat(CalendarDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneOffsetBasic);
        private static readonly string CalendarDateBasicCompleteTimeZoneOffsetHour = string.Concat(CalendarDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string CalendarDateBasicCompleteUtc = string.Concat(CalendarDateBasic, TimeDesignator, TimeOfDayBasic, TimeZoneUtc);

        private static readonly string CalendarDateExtendedComplete = string.Concat(CalendarDateExtended, TimeDesignator, TimeOfDayExtended, TimeShiftSymbol, TimeZoneOffsetExtended);
        private static readonly string CalendarDateExtendedCompleteTimeZoneOffsetHour = string.Concat(CalendarDateExtended, TimeDesignator, TimeOfDayExtended, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string CalendarDateExtendedCompleteUtc = string.Concat(CalendarDateExtended, TimeDesignator, TimeOfDayExtended, TimeZoneUtc);

        public static List<string> CalendarDateFormats => new()
        {
            CalendarDateBasic,
            CalendarDateBasicComplete,
            CalendarDateBasicCompleteTimeZoneOffsetHour,
            CalendarDateBasicCompleteUtc,
            CalendarDateExtended,
            CalendarDateExtendedComplete,
            CalendarDateExtendedCompleteTimeZoneOffsetHour,
            CalendarDateExtendedCompleteUtc,
        };


        private static readonly string OrdinalDateBasicComplete = string.Concat(OrdinalDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneOffsetBasic);
        private static readonly string OrdinalDateBasicCompleteTimeZoneOffsetHour = string.Concat(OrdinalDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string OrdinalDateBasicCompleteUtc = string.Concat(OrdinalDateBasic, TimeDesignator, TimeOfDayBasic, TimeZoneUtc);

        private static readonly string OrdinalDateExtendedComplete = string.Concat(OrdinalDateExtended, TimeDesignator, TimeOfDayExtended, TimeShiftSymbol, TimeZoneOffsetExtended);
        private static readonly string OrdinalDateExtendedCompleteTimeZoneOffsetHour = string.Concat(OrdinalDateExtended, TimeDesignator, TimeOfDayExtended, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string OrdinalDateExtendedCompleteUtc = string.Concat(OrdinalDateExtended, TimeDesignator, TimeOfDayExtended, TimeZoneUtc);

        public static List<string> OrdinalDateFormats => new()
        {
            OrdinalDateBasic,
            OrdinalDateBasicComplete,
            OrdinalDateBasicCompleteTimeZoneOffsetHour,
            OrdinalDateBasicCompleteUtc,
            OrdinalDateExtended,
            OrdinalDateExtendedComplete,
            OrdinalDateExtendedCompleteTimeZoneOffsetHour,
            OrdinalDateExtendedCompleteUtc,
        };


        private static readonly string WeekDateBasicComplete = string.Concat(WeekDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneOffsetBasic);
        private static readonly string WeekDateBasicCompleteTimeZoneOffsetHour = string.Concat(WeekDateBasic, TimeDesignator, TimeOfDayBasic, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string WeekDateBasicCompleteUtc = string.Concat(WeekDateBasic, TimeDesignator, TimeOfDayBasic, TimeZoneUtc);

        private static readonly string WeekDateExtendedComplete = string.Concat(WeekDateExtended, TimeDesignator,TimeOfDayExtended, TimeShiftSymbol, TimeZoneOffsetExtended);
        private static readonly string WeekDateExtendedCompleteTimeZoneOffsetHour = string.Concat(WeekDateExtended, TimeDesignator,TimeOfDayExtended, TimeShiftSymbol, TimeZoneHourOffset);
        private static readonly string WeekDateExtendedCompleteUtc = string.Concat(WeekDateExtended, TimeDesignator,TimeOfDayExtended, TimeZoneUtc);

        public static List<string> WeekDateFormats => new()
        {
            WeekDateBasic,
            WeekDateBasicComplete,
            WeekDateBasicCompleteTimeZoneOffsetHour,
            WeekDateBasicCompleteUtc,
            WeekDateExtended,
            WeekDateExtendedComplete,
            WeekDateExtendedCompleteTimeZoneOffsetHour,
            WeekDateExtendedCompleteUtc,
        };

        public static string ConvertToBasicRepresentation(string dateTimeString)
        {
            const string colonRegex = @"(?<=\d{2}):(?=\d{2})";
            const string hyphenRegex = @"(?<=\d{1})-(?=(\d{1}|W))";

            return Regex.Replace(Regex.Replace(dateTimeString, colonRegex, string.Empty), hyphenRegex, string.Empty);
        }
    }
}
