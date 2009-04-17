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

using System.Collections.Generic;
using System.IO;

namespace Sneal.Core.IO
{
    /// <summary>
    /// Provides file and directory services to a permanent storage device.
    /// </summary>
    /// <remarks>
    /// Use this interface in your concrete classes to be able to stub external
    /// file system dependencies and actions.
    /// </remarks>
    public interface IFileSystem
    {
        /// <summary>
        /// Deletes the specified file. An exception is not thrown if the specified file does not exist.
        /// </summary>
        /// <param name="path">The name of the file to be deleted.</param>
        void DeleteFile(string path);

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The name of the file to be deleted.</param>
        /// <returns><c>true</c> if the file exists, otherwise <c>false</c></returns>
        bool FileExists(string path);

        /// <summary>
        /// Sets the specified FileAttributes of the file on the specified path.
        /// </summary>
        /// <param name="path">The name of the file to modify.</param>
        /// <param name="attributes">The attributes to set on the file.</param>
        void SetAttributes(string path, FileAttributes attributes);

        /// <summary>
        /// Gets the FileAttributes of the file on the path.
        /// </summary>
        FileAttributes GetAttributes(string path);

        /// <summary>
        /// Creates the specified directory and any parent directories if needed.
        /// </summary>
        void CreateDirectory(string path);

        /// <summary>
        /// Creates the specified directory only if it does not exist.
        /// </summary>
        void EnsureDirectory(string path);

        /// <summary>
        /// Deletes the specified directory, subdirectories, and file.
        /// </summary>
        void DeleteDirectory(string path);

        /// <summary>
        /// Checks whether the specified directory exists.
        /// </summary>
        /// <param name="path">The path of the directory to check</param>
        /// <returns><c>true</c> if the directory exists, otherwise <c>false</c></returns>
        bool DirectoryExists(string path);

        /// <summary>
        /// Gets all the files within the specified directory.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <returns>
        /// A String array containing the names of files in the specified directory
        /// that match the specified search pattern.
        /// </returns>
        IEnumerable<string> GetFiles(string path);

        /// <summary>
        /// Gets all the files within the specified directory that match the
        /// specified search pattern.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files in path. The
        /// parameter cannot end in two periods ("..") or contain two periods
        /// ("..") followed by DirectorySeparatorChar or AltDirectorySeparatorChar, 
        /// nor can it contain any of the characters in InvalidPathChars.
        /// </param>
        /// <returns>
        /// A String array containing the names of files in the specified directory
        /// that match the specified search pattern.
        /// </returns>
        IEnumerable<string> GetFiles(string path, string searchPattern);

        /// <summary>
        /// Gets the names of subdirectories in the specified directory.
        /// </summary>
        IEnumerable<string> GetDirectories(string path);

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same
        /// name is not allowed.
        /// </summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="targetPath">
        /// The name of the destination file. This cannot be a directory or an
        /// existing file.
        /// </param>
        void CopyFile(string sourcePath, string targetPath);

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same
        /// name is not allowed.
        /// </summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="targetPath">The name of the destination file.</param>
        /// <param name="copyOption">Options for overwriting</param>
        void CopyFile(string sourcePath, string targetPath, CopyOption copyOption);

        /// <summary>
        /// Recursively copies the source directory and all its content to the
        /// specified target directory.
        /// </summary>
        /// <param name="sourcePath">The source directory to copy.</param>
        /// <param name="targetPath">The destination path.</param>
        void CopyDirectory(string sourcePath, string targetPath);

        /// <summary>
        /// Reads the entire contents of the specified file into a byte array.
        /// </summary>
        byte[] ReadAllBytes(string path);

        /// <summary>
        /// Opens a text file, reads all the lines, then closes the file.
        /// </summary>
        IEnumerable<string> ReadAllLines(string path);

        /// <summary>
        /// Opens a text file, reads all the lines, then closes the file.
        /// </summary>
        string ReadAllText(string path);

        /// <summary>
        /// Creates a new file, writes the specified byte array to the file,
        /// and then closes the file. If the target file already exists, it
        /// is overwritten.
        /// </summary>
        void WriteAllBytes(string path, byte[] contents);

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and
        /// then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        void WriteAllLines(string path, string[] contents);

        /// <summary>
        /// Creates a new file, write the contents to the file, and then closes
        /// the file. If the target file already exists, it is overwritten.
        /// </summary>
        void WriteAllText(string path, string contents);
    }
}
