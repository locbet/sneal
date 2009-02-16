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

using System;
using System.IO;

namespace Sneal.JsUnitUtils.Utils
{
    public class DiskProviderImpl : IDiskProvider
    {
        public virtual string Combine(string lhs, string rhs)
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

        public virtual string MakeHttp(string path)
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
        public virtual string NormalizePath(string path)
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

        public virtual void EnsureDirectorySeparator(ref string rhs, char dirSeparator)
        {
            if (string.IsNullOrEmpty(rhs))
                return;

            char oldSeparator = '\\';
            if (dirSeparator == '\\')
                oldSeparator = '/';

            rhs = rhs.Replace(oldSeparator, dirSeparator);
        }

        public virtual void RemoveDirectorySeparatorPrefix(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            while (path.StartsWith(dirSeparator.ToString()))
            {
                path = path.Substring(1);
            }
        }

        public virtual void RemoveDirectorySeparatorSuffix(ref string path, char dirSeparator)
        {
            if (string.IsNullOrEmpty(path))
                return;

            while (path.EndsWith(dirSeparator.ToString()))
            {
                path = path.Substring(0, path.Length - 1);
            }
        }

        /// <summary>
        /// Recursively searches a directory and its children for the given file,
        /// returning the first occurance of the file otherwise null.
        /// </summary>
        /// <param name="directory">The starting base directory</param>
        /// <param name="fileName">The file to search for</param>
        /// <returns>The full path to the file if found, otherwise null</returns>
        public virtual string FindFile(string directory, string fileName)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentException("Directory cannot be null or empty.");

            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("FileName cannot be null or empty.");

            if (Directory.GetFiles(directory, fileName).Length > 0)
            {
                return Combine(directory, fileName);
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                string filePath = FindFile(subDirectory, fileName);
                if (filePath != null)
                {
                    return filePath;
                }
            }

            return null;
        }

        public virtual string MakePathRelative(string basePath, string fullPath)
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

        public virtual void Copy(string sourceDirectory, string targetDirectory)
        {
            var source = new DirectoryInfo(sourceDirectory);
            var target = new DirectoryInfo(targetDirectory);

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into it’s new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                Copy(diSourceSubDir.FullName, nextTargetSubDir.FullName);
            }
        }

        public virtual void CreateDirectory(string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);
        }
    }
}
