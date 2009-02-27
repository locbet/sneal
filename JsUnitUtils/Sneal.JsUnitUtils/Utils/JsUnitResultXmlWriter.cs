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
using System.IO;
using System.Xml;

namespace Sneal.JsUnitUtils.Utils
{
    /// <summary>
    /// Writes a standard JSUnit server XML result.
    /// </summary>
    public class JsUnitResultXmlWriter
    {
        private XmlWriter xmlWriter;
        private IEnumerable<JsUnitErrorResult> results;

        public void WriteResults(IEnumerable<JsUnitErrorResult> testCaseResults, TextWriter writer)
        {
            results = testCaseResults;
            CreateXmlWriter(writer);

            WriteTestSuiteElementStart();
            WriteTestProperties();
            WriteTestCaseElements();
            WriteTestSuiteElementEnd();

            xmlWriter.Flush();
        }

        private void CreateXmlWriter(TextWriter writer)
        {
            var settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = Environment.NewLine
            };
            xmlWriter = XmlWriter.Create(writer, settings);
            if (xmlWriter == null)
            {
                throw new InvalidOperationException("Couldn't create xmlWriter");
            }
        }

        private void WriteTestProperties()
        {
            xmlWriter.WriteStartElement("properties");
            // TODO: write out properties here
            xmlWriter.WriteEndElement();
        }

        private void WriteTestCaseElements()
        {
            foreach (JsUnitErrorResult result in results)
            {
                WriteResult(result);
            }
        }

        private void WriteTestSuiteElementStart()
        {
            xmlWriter.WriteStartElement("testsuite");
            xmlWriter.WriteAttributeString("id", Guid.NewGuid().ToString("N"));
            xmlWriter.WriteAttributeString("errors", ErrorCount().ToString());
            xmlWriter.WriteAttributeString("failures", FailureCount().ToString());
            xmlWriter.WriteAttributeString("name", "JsUnitTest");
            xmlWriter.WriteAttributeString("tests", TestCount().ToString());
            xmlWriter.WriteAttributeString("time", FixtureTime().ToString());
        }

        private void WriteTestSuiteElementEnd()
        {
            xmlWriter.WriteEndElement();
        }

        private void WriteResult(JsUnitErrorResult result)
        {
            xmlWriter.WriteStartElement("testcase");
            xmlWriter.WriteAttributeString("name", result.FunctionName);
            xmlWriter.WriteAttributeString("testpage", result.TestPage);
            xmlWriter.WriteAttributeString("result", result.TestResult.ToString().ToLowerInvariant());
            xmlWriter.WriteAttributeString("time", result.Timing);
            if (result.IsError)
            {
                xmlWriter.WriteStartElement(result.TestResult.ToString().ToLowerInvariant());
                xmlWriter.WriteAttributeString("message", result.Message);
                xmlWriter.WriteEndElement(); // failure/error
            }
            xmlWriter.WriteEndElement(); // testcase
        }

        private float FixtureTime()
        {
            float totalTime = 0;
            foreach (JsUnitErrorResult result in results)
            {
                float time;
                if (float.TryParse(result.Timing, out time))
                {
                    totalTime += time;
                }

            }
            return totalTime;
        }

        private int TestCount()
        {
            int count = 0;
            foreach (JsUnitErrorResult result in results)
            {
                count++;
            }
            return count;
        }

        private int FailureCount()
        {
            int count = 0;
            foreach (JsUnitErrorResult result in results)
            {
                if (result.TestResult == TestResult.Failure)
                    count++;
            }
            return count;
        }

        private int ErrorCount()
        {
            int count = 0;
            foreach (JsUnitErrorResult result in results)
            {
                if (result.TestResult == TestResult.Error)
                    count++;
            }
            return count;
        }
    }
}
