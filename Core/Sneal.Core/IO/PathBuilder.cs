#region license
// Copyright 2009 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.IO;

namespace Sneal.Core.IO
{
    /// <summary>
    /// Default path builder implmentation.
    /// </summary>
    public class PathBuilder : IPathBuilder
    {
        public string Combine(params string[] paths)
        {
            string path = "";
            foreach (string pathPart in paths)
            {
                path = Combine(path, pathPart);
            }
            return path;
        }

        private string Combine(string lhs, string rhs)
        {
            if (string.IsNullOrEmpty(lhs) && string.IsNullOrEmpty(rhs))
                return "";

            if (string.IsNullOrEmpty(lhs))
                return rhs;

            if (string.IsNullOrEmpty(rhs))
                return lhs;

            char dirChar = PathSeparator(lhs);

            rhs = EnsureDirectorySeparator(rhs, dirChar);
            lhs = EnsureDirectorySeparator(lhs, dirChar);
            rhs = RemoveDirectorySeparatorPrefix(rhs);
            lhs = RemoveDirectorySeparatorSuffix(lhs);

            return lhs + dirChar + rhs;
        }

        private static char PathSeparator(string path)
        {
            char dirChar = '\\';
            if (path != null && path.Contains("/"))
            {
                dirChar = '/';
            }
            return dirChar;
        }

        public string Normalize(string path)
        {
            char dirChar = PathSeparator(path);

            path = EnsureDirectorySeparator(path, '\\');
            while (path.Contains(@"\..\"))
            {
                int idx = path.IndexOf(@"\..\");
                string lhs = path.Substring(0, idx);
                string rhs = path.Substring(idx + 4, path.Length - idx - 4);

                var lhsParts = lhs.Split('\\');
                lhs = string.Join(@"\", lhsParts, 0, lhsParts.Length - 1);

                path = Combine(lhs, rhs);
            }

            path = EnsureDirectorySeparator(path, dirChar);
            return path;
        }

        public string MakeRelative(string basePath, string fullPath)
        {
            basePath = Normalize(basePath);
            fullPath = Normalize(fullPath);

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

        public string EnsureDirectorySeparator(string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            char oldSeparator = '\\';
            if (dirSeparator == '\\')
                oldSeparator = '/';

            return path.Replace(oldSeparator, dirSeparator);
        }

        public string FileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string FileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public string FileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string DirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string RootDirectory(string path)
        {
            return Path.GetPathRoot(path);
        }

        public bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        public string ChangeScheme(string path, string newScheme)
        {
            if (string.IsNullOrEmpty(path))
            {
                return newScheme;
            }

            path = RemoveScheme(path);
            path = EnsureDirectorySeparator(path, '/');

            return newScheme + "://" + path;
        }

        private static string RemoveScheme(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }

            int colon = path.IndexOf(":");
            if (colon != -1)
            {
                path = path.Substring(colon + 1);
            }

            path = RemoveDirectorySeparatorPrefix(path);
            return path;
        }

        private static string RemoveDirectorySeparatorPrefix(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            while (path.StartsWith("\\") || path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            return path;
        }

        private static string RemoveDirectorySeparatorSuffix(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            while (path.EndsWith("\\") || path.EndsWith("/"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            return path;
        }
    }
}
