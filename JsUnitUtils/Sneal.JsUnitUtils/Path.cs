#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
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

namespace Sneal.JsUnitUtils
{
    public static class Path
    {
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

        public static string MakeHttp(string path)
        {
            string relativePath = path;

            // handle fully qualified path
            int colonIdx = path.IndexOf(':');
            if (colonIdx != -1)
            {
                relativePath = path.Substring(colonIdx+1);
            }

            EnsureDirectorySeparator(ref relativePath, '/');
            RemoveDirectorySeparatorPrefix(ref relativePath, '/');

            return "http://" + relativePath;            
        }

        /// <summary>
        /// Removes any directory \..\ elements
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

        public static void EnsureDirectorySeparator(ref string rhs, char dirSeparator)
        {
            if (string.IsNullOrEmpty(rhs))
                return;

            char oldSeparator = '\\';
            if (dirSeparator == '\\')
                oldSeparator = '/';

            rhs = rhs.Replace(oldSeparator, dirSeparator);
        }

        public static void RemoveDirectorySeparatorPrefix(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            while (path.StartsWith(dirSeparator.ToString()))
            {
                path = path.Substring(1);
            }
        }

        public static void RemoveDirectorySeparatorSuffix(ref string path, char dirSeparator)
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
