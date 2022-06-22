using System;

namespace Arkivverket.Arkade.Core.Util
{
    public class NorwegianIdNumberBase
    {

        public string _id;

        protected static string CalculateChecksumPart(string dateAndPersonNumberPart)
        {
            int[] b = StringUtil.ToIntArray(dateAndPersonNumberPart);

            int first = 11 -
                        (((3 * b[0]) + (7 * b[1]) + (6 * b[2]) + (1 * b[3]) + (8 * b[4]) + (9 * b[5]) + (4 * b[6]) + (5 * b[7]) +
                          (2 * b[8]))
                         % 11);
            first = (first == 11 ? 0 : first);


            int second = 11 -
                         (((5 * b[0]) + (4 * b[1]) + (3 * b[2]) + (2 * b[3]) + (7 * b[4]) + (6 * b[5]) + (5 * b[6]) + (4 * b[7]) +
                           (3 * b[8]) +
                           (2 * first)) % 11);
            second = (second == 11 ? 0 : second);

            return first.ToString() + second.ToString();
        }

        protected static bool DateExists(string date)
        {
            try
            {
                DateTime.ParseExact(date, "ddMMyy", System.Globalization.CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        protected static string StripSpace(string s)
        {
            return s.Replace(" ", "");
        }

        public override int GetHashCode()
        {
            return _id?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return _id;
        }
    }
}