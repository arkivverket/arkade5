using System;

namespace Arkivverket.Arkade.Core.Util
{
    public class StringUtil
    {
        public static int? ToInt(string s)
        {
            int ret;
            return int.TryParse(s, out ret) ? ret : (int?) null;
        }

        public static int[] ToIntArray(string stringWithInts)
        {
            int[] intArray = new int[stringWithInts.Length];

            char[] charArray = stringWithInts.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                intArray[i] = Convert.ToInt32(charArray[i].ToString());
            }

            return intArray;
        }

        public static bool IsOnlyDigits(string s)
        {
            foreach (char c in s)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }

            return true;
        }

        public static string WhiteSpaceToEscaped(string s)
        {
            return s
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                ;
        }
    }
}