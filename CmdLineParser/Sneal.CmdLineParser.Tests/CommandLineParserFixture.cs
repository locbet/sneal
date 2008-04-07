using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sneal.CmdLineParser;

namespace Sneal.CmdLineParser.Tests
{
    [TestFixture]
    public class CommandLineParserFixture
    {
        private TestOptions testOptions;
        private CommandLineParser parser;

        [SetUp]
        public void SetUp()
        {
            testOptions = new TestOptions();
        }

        [Test]
        public void ShouldGetSettableProperties()
        {
            parser = new CommandLineParser(new string[] { "" });

            Dictionary<string, CommandLineParser.PropertyInfoSwitchAttributePair> settableOptions = 
                parser.GetSettableOptions(testOptions);

            Assert.IsNotNull(settableOptions);
            Assert.That(settableOptions.Count == 3);

            Assert.That(settableOptions.ContainsKey("StringOption"));
            Assert.That(settableOptions.ContainsKey("BoolOption"));
            Assert.That(settableOptions.ContainsKey("IntOption"));
        }

        [Test]
        public void ShouldSetOptions()
        {
            string[] cmdLineArgs = {"/StringOption=Sneal", "/BoolOption", "/IntOptions=22"};
            parser = new CommandLineParser(cmdLineArgs);

            testOptions = parser.BuildOptions(testOptions);

            Assert.AreEqual("Sneal", testOptions.StringOption);
            Assert.IsTrue(testOptions.BoolOption);
            Assert.AreEqual(22, testOptions.IntOption);
        }

        [Test]
        public void ShouldSetBoolOptionExplicit()
        {
            string[] cmdLineArgs = { "/BoolOption=true" };
            parser = new CommandLineParser(cmdLineArgs);

            testOptions = parser.BuildOptions(testOptions);

            Assert.IsTrue(testOptions.BoolOption);
        }

        [Test]
        [ExpectedException(typeof(CmdLineParserException))]
        public void ShouldRequireValueForString()
        {
            string[] cmdLineArgs = { "/StringOption" };
            parser = new CommandLineParser(cmdLineArgs);

            parser.BuildOptions(testOptions);
        }

        [Test]
        [ExpectedException(typeof(CmdLineParserException))]
        public void ShouldRequireValueForInt()
        {
            string[] cmdLineArgs = { "/IntOption" };
            parser = new CommandLineParser(cmdLineArgs);

            parser.BuildOptions(testOptions);
        }

        [Test]
        public void ShouldGetUsageLines()
        {
            IList<string> lines = CommandLineParser.GetUsageLines(testOptions);

            foreach (string line in lines)
                Console.WriteLine(line);

            Assert.That(lines.Count == 3);
        }
    }

    internal class TestOptions
    {
        private bool boolOption;
        private int intOption;
        private string stringOption;

        [Switch("StringOption", "Some sort of string option")]
        public string StringOption
        {
            get { return stringOption; }
            set { stringOption = value; }
        }

        [Switch("BoolOption", "Some sort of bool option")]
        public bool BoolOption
        {
            get { return boolOption; }
            set { boolOption = value; }
        }

        [Switch("IntOption", "Some sort of int option")]
        public int IntOption
        {
            get { return intOption; }
            set { intOption = value; }
        }
    }
}