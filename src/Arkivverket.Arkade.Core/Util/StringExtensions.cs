namespace Arkivverket.Arkade.Core.Util
{
    public static class StringExtensions
    {
        public static string ForwardSlashed(this string text)
        {
            return text.Replace('\\', '/');
        }
    }
}
