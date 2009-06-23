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

namespace Sneal.Core.IO
{
    /// <summary>
    /// Constructs and or modifies a file system path.
    /// </summary>
    public interface IPathBuilder
    {
        /// <summary>
        /// Safely combines a Windows path, *nix path, or Uri.  The
        /// directory separator from the first path argument is used to combine
        /// the parts and replaces any separators in the rhs.
        /// </summary>
        /// <param name="paths">The path parts to join</param>
        /// <returns>The new combined path</returns>
        string Combine(params string[] paths);

        /// <summary>
        /// Removes any relative directory \..\ elements
        /// </summary>
        /// <param name="path">The path to act on.</param>
        string Normalize(string path);

        /// <summary>
        /// Returns the relative part between two paths.
        /// </summary>
        /// <param name="basePath">The shorter, base path.</param>
        /// <param name="fullPath">The longer full path, which is a child of basePath</param>
        /// <returns>The relative part of the path</returns>
        string MakeRelative(string basePath, string fullPath);

        /// <summary>
        /// Ensures a given path is using the specified directory separator.
        /// </summary>
        /// <param name="path">The path string</param>
        /// <param name="dirSeparator">The directory separator to use</param>
        string EnsureDirectorySeparator(string path, char dirSeparator);

        /// <summary>
        /// Returns the file name and extension of the specified path string.
        /// </summary>
        /// <param name="path">The path string</param>
        string FileName(string path);

        /// <summary>
        /// Returns the file name of the specified path string without the extension.
        /// </summary>
        /// <param name="path">The path string</param>
        string FileNameWithoutExtension(string path);

        /// <summary>
        /// Returns the extension of the specified path string.
        /// </summary>
        /// <param name="path">The path string</param>
        /// <returns></returns>
        string FileExtension(string path);

        /// <summary>
        /// Returns the directory information for the specified path string
        /// </summary>
        /// <param name="path">The path string</param>
        string DirectoryName(string path);

        /// <summary>
        /// Gets the root directory information of the specified path, i.e. c:\
        /// </summary>
        /// <param name="path">The path string</param>
        /// <returns>The drive</returns>
        string RootDirectory(string path);

        /// <summary>
        /// Gets a value indicating whether the specified path string contains
        /// absolute or relative path information.
        /// </summary>
        /// <param name="path">The path string</param>
        /// <returns><c>true</c> for fully qualified paths</returns>
        bool IsPathRooted(string path);

        /// <summary>
        /// Replaces the path scheme to the specified new scheme.
        /// </summary>
        /// <param name="path">The path to change</param>
        /// <param name="newScheme">The new scheme, i.e. http://</param>
        /// <returns>The path with the new scheme</returns>
        string ChangeScheme(string path, string newScheme);
    }
}
