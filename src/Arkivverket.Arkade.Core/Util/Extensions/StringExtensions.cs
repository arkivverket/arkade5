namespace Arkivverket.Arkade.Core.Util
{
    public static class StringExtensions
    {
        public static string ForwardSlashed(this string text)
        {
            return text.Replace('\\', '/');
        }


        /// <summary>
        /// Removes a single leading and trailing occurrence of a specified <paramref name="value"/> from the current
        /// string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns>
        /// The string that remains after a single occurrence of the <paramref name="value"/> parameter are removed
        /// from the start and end of the current string. If <paramref name="value"/> is null or empty, or if the
        /// current instance does not start or end with <paramref name="value"/>, the method returns the current instance
        /// unchanged.
        /// </returns>
        public static string Trim(this string text, string value)
        {
            return text.TrimStart(value).TrimEnd(value);
        }

        /// <summary>
        /// Removes a single leading occurrence of a specified <paramref name="value"/> from the current string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns>
        /// The string that remains after a single occurrence of the <paramref name="value"/> parameter are removed
        /// from the start of the current string. If <paramref name="value"/> is null or empty, or if the current
        /// instance does not start with <paramref name="value"/>, the method returns the current instance unchanged.
        /// </returns>
        public static string TrimStart(this string text, string value)
        {
            if (string.IsNullOrEmpty(value))
                return text;
            return text.StartsWith(value) ? text[value.Length..] : text;
        }

        /// <summary>
        /// Removes a single trailing occurrence of a specified <paramref name="value"/> from the current string.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns>
        /// The string that remains after a single occurrence of the <paramref name="value"/> parameter are removed
        /// from the end of the current string. If <paramref name="value"/> is null or empty, or if the current
        /// instance does not end with <paramref name="value"/>, the method returns the current instance unchanged.
        /// </returns>
        public static string TrimEnd(this string text, string value)
        {
            if (string.IsNullOrEmpty(value))
                return text;
            return text.EndsWith(value) ? text[..^value.Length] : text;
        }
    }
}
