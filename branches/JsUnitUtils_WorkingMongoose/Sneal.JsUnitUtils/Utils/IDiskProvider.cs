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

using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils.Utils
{
    /// <summary>
    /// Wraps access to a physical disk storage medium and provides utilities
    /// for accessing the file system.
    /// </summary>
    public interface IDiskProvider
    {
        /// <summary>
        /// Recursively searches a directory and its children for the given file,
        /// returning the first occurance of the file otherwise null.
        /// </summary>
        /// <param name="directory">The starting base directory</param>
        /// <param name="fileName">The file to search for</param>
        /// <returns>The full path to the file if found, otherwise null</returns>
        string FindFile([NotNullOrEmpty] string directory, [NotNullOrEmpty] string fileName);

        /// <summary>
        /// Recursively copies the specified source directory and contents to the
        /// specified target directory.  If the target directory does not exist
        /// this will create it first.
        /// </summary>
        /// <param name="sourceDirectory">The path to the source directory</param>
        /// <param name="targetDirectory">The target path</param>
        void Copy([NotNullOrEmpty] string sourceDirectory, [NotNullOrEmpty] string targetDirectory);

        /// <summary>
        /// Creates a directory, and any required parent directories on disk.
        /// </summary>
        /// <param name="directoryPath">The full path to the directory to create</param>
        void CreateDirectory([NotNullOrEmpty] string directoryPath);

        /// <summary>
        /// Makes the full path relative to the base path.  Turns
        /// c:\src\myproj, c:\src\myproj\dir1 returns dir1
        /// </summary>
        /// <param name="basePath">The shorter base path</param>
        /// <param name="fullPath">The full path, which includes the base path.</param>
        /// <returns>The full path relative to the base path.</returns>
        string MakePathRelative([NotNull] string basePath, [NotNull] string fullPath);

        /// <summary>
        /// Combines two path portions safely, adding/removing any extra directory
        /// separator characaters.
        /// </summary>
        /// <param name="lhs">The first or left side of the path string.</param>
        /// <param name="rhs">The right side of the path string.</param>
        /// <returns>The combined path.</returns>
        string Combine(string lhs, string rhs);

        /// <summary>
        /// Converts a local UNC or drive and folder path to an HTTP URI.  If
        /// the path is already an HTTP URI, the original string is returned.
        /// </summary>
        /// <param name="path">The path to convert</param>
        /// <returns>The Http path.</returns>
        string MakeHttp([NotNullOrEmpty] string path);

        /// <summary>
        /// Removes any directory \..\ elements
        /// </summary>
        /// <param name="path"></param>
        string NormalizePath([NotNull] string path);

        void EnsureDirectorySeparator([NotNull] ref string rhs, char dirSeparator);

        void RemoveDirectorySeparatorPrefix([NotNull] ref string path, char dirSeparator);

        void RemoveDirectorySeparatorSuffix([NotNull] ref string path, char dirSeparator);
    }
}