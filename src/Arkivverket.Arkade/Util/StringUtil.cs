namespace Arkivverket.Arkade.Util
{
    public class StringUtil
    {
        public static int? ToInt(string s)
        {
            int ret;
            return int.TryParse(s, out ret) ? ret : (int?) null;
        }
    }
}