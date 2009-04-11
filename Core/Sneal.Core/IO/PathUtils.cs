

using System;

namespace Sneal.Core.IO
{
    /// <summary>
    /// Adds functionality misssing from System.IO.Path
    /// </summary>
    public static class PathUtils
    {
        /// <summary>
        /// Safely combines a Windows path, *nix path, or HTTP path.  The
        /// directory separator from the lhs is used to combine the parts
        /// and replaces any separators in the rhs.
        /// </summary>
        /// <param name="lhs">The root, or left side, of the path</param>
        /// <param name="rhs">The part to add</param>
        /// <returns>The new combined path</returns>
        public static string Combine(string lhs, string rhs)
        {
            if (string.IsNullOrEmpty(lhs) && string.IsNullOrEmpty(rhs))
                return "";

            if (string.IsNullOrEmpty(lhs))
                return rhs;

            if (string.IsNullOrEmpty(rhs))
                return lhs;

            char dirChar = '\\';
            if (lhs.Contains("/"))
                dirChar = '/';

            EnsureDirectorySeparator(ref rhs, dirChar);
            EnsureDirectorySeparator(ref lhs, dirChar);
            RemoveDirectorySeparatorSuffix(ref lhs, dirChar);
            RemoveDirectorySeparatorPrefix(ref rhs, dirChar);

            return lhs + dirChar + rhs;
        }

        /// <summary>
        /// Turns the given local path into an HTTP path, or if already HTTP
        /// the given path is returned.
        /// </summary>
        /// <param name="path">The path to make HTTP</param>
        /// <returns>The path as an HTTP URI</returns>
        public static string MakePathHttp(string path)
        {
            string relativePath = path;

            // handle fully qualified path
            int colonIdx = path.IndexOf(':');
            if (colonIdx != -1)
            {
                relativePath = path.Substring(colonIdx + 1);
            }

            EnsureDirectorySeparator(ref relativePath, '/');
            RemoveDirectorySeparatorPrefix(ref relativePath, '/');

            return "http://" + relativePath;
        }

        /// <summary>
        /// Removes any relative directory \..\ elements
        /// </summary>
        /// <param name="path"></param>
        public static string NormalizePath(string path)
        {
            EnsureDirectorySeparator(ref path, '\\');
            while (path.Contains(@"\..\"))
            {
                int idx = path.IndexOf(@"\..\");
                string lhs = path.Substring(0, idx);
                string rhs = path.Substring(idx + 4, path.Length - idx - 4);

                var lhsParts = lhs.Split('\\');
                lhs = string.Join(@"\", lhsParts, 0, lhsParts.Length - 1);

                path = Combine(lhs, rhs);
            }

            return path;
        }

        /// <summary>
        /// Returns the relative part between two paths.
        /// </summary>
        /// <param name="basePath">The shorter, base path.</param>
        /// <param name="fullPath">The longer full path, which is a child of basePath</param>
        /// <returns>The relative part of the path</returns>
        public static string MakePathRelative(string basePath, string fullPath)
        {
            basePath = NormalizePath(basePath);
            fullPath = NormalizePath(fullPath);

            if (basePath.Length > fullPath.Length)
            {
                throw new ArgumentOutOfRangeException("basePath",
                    "The base path cannot be longer than the full path.");
            }

            if (!fullPath.ToLowerInvariant().Contains(basePath.ToLowerInvariant()))
            {
                throw new ArgumentException("The full path must be a subdirectory of the base path.");
            }

            return fullPath.Substring(basePath.Length);
        }

        /// <summary>
        /// Ensures a given path is using the specified directory separator.
        /// </summary>
        /// <param name="path">The path string</param>
        /// <param name="dirSeparator">The directory separator to use</param>
        public static void EnsureDirectorySeparator(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            char oldSeparator = '\\';
            if (dirSeparator == '\\')
                oldSeparator = '/';

            path = path.Replace(oldSeparator, dirSeparator);
        }

        private static void RemoveDirectorySeparatorPrefix(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            while (path.StartsWith(dirSeparator.ToString()))
            {
                path = path.Substring(1);
            }
        }

        private static void RemoveDirectorySeparatorSuffix(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            while (path.EndsWith(dirSeparator.ToString()))
            {
                path = path.Substring(0, path.Length - 1);
            }
        }
    }
}
