namespace Arkivverket.Arkade.Core.Util
{
    public static class PathUtil
    {
        /// <summary>
        /// Will try to merge <paramref name="path1"/> with <paramref name="path2"/>.
        /// </summary>
        /// <param name="path1"></param>
        /// <param name="path2"></param>
        /// <returns>
        /// If <paramref name="path1"/> is null or empty, returns <paramref name="path2"/>.
        /// If <paramref name="path1"/> contains <paramref name="path2"/> or <paramref name="path2"/> is the empty string (""), returns <paramref name="path1"/>.
        /// Else, returns <paramref name="path2"/> + "/" + <paramref name="path1"/>
        /// </returns>
        public static string Merge(string path1, string path2)
        {
            if (string.IsNullOrWhiteSpace(path1))
                return path2;

            if (path1.Contains(path2))
                return path1;

            return path2 + "/" + path1;
        }
    }
}
