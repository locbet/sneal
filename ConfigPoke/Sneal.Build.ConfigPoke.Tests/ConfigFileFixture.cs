#region license
// Copyright 2008 Shawn Neal (neal.shawn@gmail.com)
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

using System.IO;
using NUnit.Framework;

namespace Sneal.Build.ConfigPoke.Tests
{
    [TestFixture]
    public class ConfigFileFixture
    {
        private string configDir;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            configDir = @"..\..\Configs\";
        }

        [SetUp]
        public void SetUp()
        {
            if (File.Exists("web.config"))
                File.Delete("web.config");
        }

        [Test]
        public void ShouldReplacePropertyPlaceholdersWithLiveEnvironmentValues()
        {
            ConfigFile config = new ConfigFile(configDir + "web.config.template");
            ConfigPropertiesCollection liveProperties = new ConfigPropertiesCollection();

            using (ConfigPropertiesReader defaultPropReader = new ConfigPropertiesReader(configDir + "app.properties"))
            {
                using (ConfigPropertiesReader livePropReader = new ConfigPropertiesReader(configDir + "app.properties.live"))
                {
                    liveProperties.AddPropertiesFromReader(defaultPropReader);
                    liveProperties.AddPropertiesFromReader(livePropReader);
                }
            }

            config.ReplacePropertyPlaceholdersWith(liveProperties).SaveAs("web.config");

            Assert.IsTrue(File.Exists("web.config"), "web.config file is missing");
            using (StreamReader reader = File.OpenText("web.config"))
            {
                string contents = reader.ReadToEnd();
                Assert.That(
                    contents.Contains(
                        "<add name=\"Northwind\" connectionString=\"server=live;database=Northwind;integrated security=SSPI\"/>"),
                    "missing northwind connection string");
                Assert.That(
                    contents.Contains("sqlConnectionString=\"server=live;database=AspState;integrated security=SSPI\""),
                    "missing session state connection string");
            }
        }

        [Test]
        public void ShouldReplacePropertyPlaceholdersWithDefaultEnvironmentValues()
        {
            ConfigFile config = new ConfigFile(configDir + "web.config.template");
            ConfigPropertiesCollection properties = new ConfigPropertiesCollection();

            using (ConfigPropertiesReader defaultPropReader = new ConfigPropertiesReader(configDir + "app.properties"))
            {
                properties.AddPropertiesFromReader(defaultPropReader);
            }

            config.ReplacePropertyPlaceholdersWith(properties).SaveAs("web.config");

            Assert.IsTrue(File.Exists("web.config"), "web.config file is missing");
            using (StreamReader reader = File.OpenText("web.config"))
            {
                string contents = reader.ReadToEnd();
                Assert.That(
                    contents.Contains(
                        "<add name=\"Northwind\" connectionString=\"server=localhost;database=Northwind;integrated security=SSPI\"/>"),
                    "missing northwind connection string");
                Assert.That(
                    contents.Contains(
                        "sqlConnectionString=\"server=localhost;database=AspState;integrated security=SSPI\""),
                    "missing session state connection string");
            }
        }

        [Test]
        public void ShouldReplacePropertyPlaceholdersWithDefaultEnvironmentValuesForDbConnectionString()
        {
            ConfigFile config = new ConfigFile(configDir + "web.config.template");
            ConfigPropertiesCollection liveProperties = new ConfigPropertiesCollection();

            using (ConfigPropertiesReader defaultPropReader = new ConfigPropertiesReader(configDir + "app.properties"))
            {
                using (ConfigPropertiesReader devReader = new ConfigPropertiesReader(configDir + "app.properties.dev2"))
                {
                    liveProperties.AddPropertiesFromReader(defaultPropReader).AddPropertiesFromReader(devReader);
                }
            }

            config.ReplacePropertyPlaceholdersWith(liveProperties).SaveAs("web.config");

            Assert.IsTrue(File.Exists("web.config"), "web.config file is missing");
            using (StreamReader reader = File.OpenText("web.config"))
            {
                string contents = reader.ReadToEnd();
                Assert.That(
                    contents.Contains(
                        "<add name=\"Northwind\" connectionString=\"server=localhost;database=Northwind;integrated security=SSPI\"/>"),
                    "missing northwind connection string from app.properties");
                Assert.That(
                    contents.Contains(
                        "sqlConnectionString=\"server=mylaptop;database=AspState;integrated security=SSPI\""),
                    "missing session state connection string from app.properties.dev2");
            }
        }
    }
}