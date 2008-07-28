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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.IO;
using System.Reflection;

namespace Sneal.JsUnitUtils
{
    public class Templates : ITemplates
    {
        // Fields
        private readonly string installDirectory;

        // Methods
        public Templates(string jsUnitInstallDirectory)
        {
            if (!Path.IsPathRooted(jsUnitInstallDirectory))
            {
                if (!jsUnitInstallDirectory.EndsWith(@"\"))
                {
                    jsUnitInstallDirectory = jsUnitInstallDirectory + @"\";
                }
                jsUnitInstallDirectory = AppDomain.CurrentDomain.BaseDirectory + @"\" + jsUnitInstallDirectory;
            }
            if (!Directory.Exists(jsUnitInstallDirectory))
            {
                throw new DirectoryNotFoundException(string.Format("Could not find the JSUnit directory: '{0}'", jsUnitInstallDirectory));
            }
            this.installDirectory = jsUnitInstallDirectory;
        }

        public void CreateAshxHandler(string handlerFileName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("JsUnit.Utils." + handlerFileName))
            {
                using (StreamWriter writer = File.CreateText(handlerFileName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        writer.Write(reader.ReadToEnd());
                    }
                }
            }
        }

        public string CreateExposedTestFunctionBlock(string jsArray)
        {
            return string.Format(this.ResourceManager.GetString("ExposedTestFunctionBlock"), jsArray);
        }

        public string CreateExposedTestFunctionEntry(int index, string functionName)
        {
            return string.Format(this.ResourceManager.GetString("ExposedTestFunctionEntry"), index, functionName);
        }

        public string CreateJavaScriptInclude(string file)
        {
            if (!Path.IsPathRooted(file))
            {
                file = AppDomain.CurrentDomain.BaseDirectory + @"\" + file;
            }
            return string.Format(this.ResourceManager.GetString("Entry"), file);
        }

        public string CreateJsUnitCoreImport()
        {
            return this.CreateJsUnitCoreImport(this.installDirectory);
        }

        public string CreateJsUnitCoreImport(string installDir)
        {
            return string.Format(this.ResourceManager.GetString("JsUnitCore"), installDir);
        }

        public string CreateJsUnitUri(string allTestFile)
        {
            return this.CreateJsUnitUri(this.installDirectory, allTestFile);
        }

        public string CreateJsUnitUri(string installDir, string allTestFile)
        {
            return string.Format(this.ResourceManager.GetString("JsUnitTestRunner"), installDir, allTestFile);
        }

        public string CreateSuite(string testPageEntries)
        {
            return this.CreateSuite(this.installDirectory, testPageEntries);
        }

        public string CreateSuite(string importedScripts, string testFunctionEntries)
        {
            return string.Format(this.ResourceManager.GetString("Suite"), this.CreateJsUnitCoreImport(this.installDirectory), importedScripts, testFunctionEntries);
        }

        // Properties
        public ResourceManager ResourceManager
        {
            get
            {
                return new ResourceManager("JsUnit.Utils.Templates", typeof(Templates).Assembly);
            }
        }
    }

 

}
