using System;

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

        public static string GetSubPath(string cutoff, string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(cutoff))
                return null;

            cutoff = cutoff.Replace('\\', '/').Trim('/');

            int cutoffIndex = path.Replace('\\', '/').IndexOf(cutoff, StringComparison.Ordinal);

            if (cutoffIndex == -1)
                return null;

            string subPath = path[(cutoffIndex + cutoff.Length)..].TrimStart('/', '\\');

            return string.IsNullOrEmpty(subPath) ? null : subPath;
        }

        public static string GetChild(string parent, string path)
        {
            if (path is null)
                throw new ArgumentNullException(nameof(path));

            string[] pathSegments = path.Replace('\\', '/').Trim('/').Split('/');

            int indexOfParent = Array.IndexOf(pathSegments, parent);

            return indexOfParent < 0 || indexOfParent + 1 > pathSegments.Length - 1
                ? null
                : pathSegments[indexOfParent + 1];
        }
    }
}
