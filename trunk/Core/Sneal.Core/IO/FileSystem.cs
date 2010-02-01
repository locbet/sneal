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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sneal.Core.IO
{
    /// <summary>
    /// Default file system implmentation for file and directory access.
    /// </summary>
    /// <remarks>
    /// Uses the default BCL file and directory IO methods wherever possible.
    /// </remarks>
    public class FileSystem : IFileSystem
    {
        private static readonly IPathBuilder PathBuilder = new PathBuilder();

        /// <summary>
        /// Deletes the specified file. An exception is not thrown if the specified file does not exist.
        /// </summary>
        /// <param name="path">The name of the file to be deleted.</param>
        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="path">The name of the file to be deleted.</param>
        /// <returns><c>true</c> if the file exists, otherwise <c>false</c></returns>
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Sets the specified FileAttributes of the file on the specified path.
        /// </summary>
        /// <param name="path">The name of the file to modify.</param>
        /// <param name="attributes">The attributes to set on the file.</param>
        public void SetAttributes(string path, FileAttributes attributes)
        {
            File.SetAttributes(path, attributes);
        }

        /// <summary>
        /// Gets the FileAttributes of the file on the path.
        /// </summary>
        public FileAttributes GetAttributes(string path)
        {
            return File.GetAttributes(path);
        }

        /// <summary>
        /// Creates the specified directory and any parent directories if needed.
        /// </summary>
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Creates the specified directory only if it does not exist.
        /// </summary>
        public void EnsureDirectory(string path)
        {
            if (!DirectoryExists(path))
            {
                CreateDirectory(path);
            }
        }

        /// <summary>
        /// Deletes the specified directory, subdirectories, and file.
        /// </summary>
        public void DeleteDirectory(string path)
        {
            Directory.Delete(path);
        }

        /// <summary>
        /// Checks whether the specified directory exists.
        /// </summary>
        /// <param name="path">The path of the directory to check</param>
        /// <returns><c>true</c> if the directory exists, otherwise <c>false</c></returns>
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Gets all the files within the specified directory.
        /// </summary>
        /// <param name="path">The directory to search.</param>
        /// <returns>
        /// A String array containing the names of files in the specified directory
        /// that match the specified search pattern.
        /// </returns>
        public IEnumerable<string> GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

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
        public IEnumerable<string> GetFiles(string path, string searchPattern)
        {
            return Directory.GetFiles(path, searchPattern);
        }

        /// <summary>
        /// Gets the names of subdirectories in the specified directory.
        /// </summary>
        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same
        /// name is not allowed.
        /// </summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="targetPath">
        /// The name of the destination file. This cannot be a directory or an
        /// existing file.
        /// </param>
        public void CopyFile(string sourcePath, string targetPath)
        {
            CopyFile(sourcePath, targetPath, CopyOption.DoNotOverwrite);
        }

        /// <summary>
        /// Copies an existing file to a new file. Overwriting a file of the same
        /// name is not allowed.
        /// </summary>
        /// <param name="sourcePath">The file to copy.</param>
        /// <param name="targetPath">The name of the destination file.</param>
        /// <param name="copyOption">Options for overwriting</param>
        public void CopyFile(string sourcePath, string targetPath, CopyOption copyOption)
        {
            if (copyOption == CopyOption.Overwrite && FileExists(targetPath))
            {
                SetAttributes(targetPath, FileAttributes.Normal);
            }
            File.Copy(sourcePath, targetPath, copyOption == CopyOption.Overwrite);
        }

        /// <summary>
        /// Recursively copies the source directory and all its content to the
        /// specified target directory.
        /// </summary>
        /// <param name="sourcePath">The source directory to copy.</param>
        /// <param name="targetPath">The destination path.</param>
        public void CopyDirectory(string sourcePath, string targetPath)
        {
            EnsureDirectory(targetPath);
            foreach (string sourceFile in GetFiles(sourcePath))
            {
                string targetFile = PathBuilder.Combine(targetPath, Path.GetFileName(sourceFile));
                CopyFile(sourceFile, targetFile, CopyOption.Overwrite);
            }
            foreach (string directory in GetDirectories(sourcePath))
            {
                CopyDirectory(directory, targetPath);
            }
        }

        /// <summary>
        /// Reads the entire contents of the specified file into a byte array.
        /// </summary>
        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }

        /// <summary>
        /// Opens a text file, reads all the lines, then closes the file.
        /// </summary>
        public IEnumerable<string> ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// Opens a text file, reads all the lines, then closes the file.
        /// </summary>
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// Creates a new file, writes the specified byte array to the file,
        /// and then closes the file. If the target file already exists, it
        /// is overwritten.
        /// </summary>
        public void WriteAllBytes(string path, byte[] contents)
        {
            File.WriteAllBytes(path, contents);
        }

        /// <summary>
        /// Creates a new file, writes the specified string to the file, and
        /// then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        public void WriteAllLines(string path, string[] contents)
        {
            File.WriteAllLines(path, contents);
        }

        /// <summary>
        /// Creates a new file, write the contents to the file, and then closes
        /// the file. If the target file already exists, it is overwritten.
        /// </summary>
        public void WriteAllText(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        /// <summary>
        /// Recursively searches a directory and its children for the given file,
        /// returning the first occurance of the file, otherwise null.
        /// </summary>
        /// <param name="directory">The starting base directory</param>
        /// <param name="fileName">The file to search for</param>
        /// <returns>The full path to the file if found, otherwise null</returns>
        public string FindFile(string directory, string fileName)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException("directory cannot be null or empty");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty");
            }

            if (Directory.GetFiles(directory, fileName).Length > 0)
            {
                return PathBuilder.Combine(directory, fileName);
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

        /// <summary>
        /// Iteratively searches a directory and its parents for the given file,
        /// returning the first occurance of the file, otherwise null.
        /// </summary>
        /// <param name="directory">The starting leaf directory</param>
        /// <param name="fileName">The file to search for</param>
        /// <returns>The full path to the file if found, otherwise null</returns>
        public string FindFileInParent(string directory, string fileName)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException("directory cannot be null or empty");
            }
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty");
            }

            string[] dirParts = directory.Split('\\');
            for (int i = dirParts.Length; i > 0; i--)
            {
                string path = string.Join("\\", dirParts.Take(i).ToArray());
                string slnPath = PathBuilder.Combine(path, fileName);
                if (FileExists(slnPath))
                {
                    return path;
                }
            }
            return null;
        }
    }
}
