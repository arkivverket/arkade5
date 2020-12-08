using System.Text;

namespace Arkivverket.Arkade.Core.Base
{
    public class Encodings
    {
        public static readonly Encoding UTF8 = Encoding.UTF8;
        public static readonly Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");

        public static Encoding GetEncoding(string charset)
        {
            return Encoding.GetEncoding(Sanitize(charset));
        }

        private static string Sanitize(string charset)
        {
            return charset.Replace("_", "-").Trim().ToLower();
        }
    }
}