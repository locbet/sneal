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

using System.Resources;
using System.IO;
using System.Reflection;

namespace Sneal.JsUnitUtils
{
    public class Templates : ITemplates
    {
        public string AshxHandlerFileName
        {
            get { return "JsUnitResultHandler.ashx"; }
        }

        public string GetAshxHandlerContent()
        {
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Sneal.JsUnitUtils." + AshxHandlerFileName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public ResourceManager ResourceManager
        {
            get
            {
                return new ResourceManager("Sneal.JsUnitUtils.Templates", typeof(Templates).Assembly);
            }
        }
    }
}
