using System;
using System.Text;

namespace Arkivverket.Arkade.Core
{
    public class Encodings
    {
        public static readonly Encoding UTF8 = Encoding.UTF8;
        public static readonly Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");
        public static readonly Encoding ISO_8859_4 = Encoding.GetEncoding("ISO-8859-4");

        public static Encoding GetEncoding(string charset)
        {
            return Encoding.GetEncoding(Sanitize(charset));
        }

        private static String Sanitize(string charset)
        {
            return charset.Replace("_", "-").Trim().ToLower();
        }
    }
}