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
using NUnit.Framework;
using Sneal.Core.IO;

namespace Sneal.Core.Tests.IO
{
    [TestFixture]
    public class FileSystemTests
    {
        private IFileSystem _fileSystem = new FileSystem();
        private List<string> _tempFiles = new List<string>();

        [TestFixtureTearDown]
        public void CleanUp()
        {
            foreach (string file in _tempFiles)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception) { }
            }
        }

        [Test]
        public void Should_copy_file()
        {
            string srcFileName = CreateTextFile();
            string destFileName = GetNewFileName();
            _fileSystem.CopyFile(srcFileName, destFileName);
            AssertFileExists(destFileName);
        }

        [Test]
        public void Should_delete_file()
        {
            string textFile = CreateTextFile();
            _fileSystem.DeleteFile(textFile);
            AssertFileDoesNotExist(textFile);
        }

        [Test]
        public void File_exists()
        {
            string textFile = CreateTextFile();
            Assert.IsTrue(_fileSystem.FileExists(textFile));
        }

        [Test]
        public void File_does_not_exist()
        {
            string textFile = GetNewFileName();
            Assert.IsFalse(_fileSystem.FileExists(textFile));
        }

        private string CreateTextFile()
        {
            string srcFile = GetNewFileName();
            File.WriteAllText(srcFile, "test file contents");
            AssertFileExists(srcFile);
            return srcFile;
        }

        private string GetNewFileName()
        {
            string newFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            _tempFiles.Add(newFileName);
            return newFileName;
        }

        private static void AssertFileExists(string file)
        {
            Assert.IsTrue(File.Exists(file),
                string.Format("Expected {0} to exist", file));
        }

        private static void AssertFileDoesNotExist(string file)
        {
            Assert.IsFalse(File.Exists(file),
                string.Format("Expected {0} to not exist", file));
        }
    }
}
