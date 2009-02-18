﻿#region license
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

using NUnit.Framework;

namespace Sneal.JsUnitUtils.Tests
{
    [TestFixture]
    public class JsUnitResultParserTests
    {
        private JsUnitResultParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new JsUnitResultParser();
        }

        [Test]
        public void Returns_error_when_result_is_null()
        {
            var results = parser.ParseJsUnitErrors(null);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void Should_return_single_failure()
        {
            const string errorResponse =
                "http://localhost/JsUnitTestFixture1.htm:testFailingTest1|0|F|\"This test should fail\"";

            var results = parser.ParseJsUnitErrors(errorResponse);
            Assert.AreEqual(1, results.Count);

            Assert.AreEqual("http://localhost/JsUnitTestFixture1.htm", results[0].TestPage);
            Assert.AreEqual("testFailingTest1", results[0].FunctionName);
            Assert.AreEqual("\"This test should fail\"", results[0].Message);
            Assert.AreEqual("0", results[0].Timing);
            Assert.AreEqual(TestResult.Failure, results[0].TestResult);
            Assert.IsTrue(results[0].IsError, "IsError should be true");
        }

        [Test, Ignore("The string split call is not good enough")]
        public void Should_parse_failure_with_comma_in_message()
        {
            const string errorResponse =
                "http://localhost/JsUnitTestFixture1.htm:testFailingTest1|0|F|\"This test, should fail\"";

            var results = parser.ParseJsUnitErrors(errorResponse);
            Assert.AreEqual(1, results.Count);

            Assert.AreEqual("http://localhost/JsUnitTestFixture1.htm", results[0].TestPage);
            Assert.AreEqual("testFailingTest1", results[0].FunctionName);
            Assert.AreEqual("\"This test, should fail\"", results[0].Message);
            Assert.AreEqual("0", results[0].Timing);
            Assert.AreEqual(TestResult.Failure, results[0].TestResult);
            Assert.IsTrue(results[0].IsError, "IsError should be true");
        }

        [Test]
        public void Should_return_two_failures()
        {
            const string errorResponse =
                "http://localhost:80/JsUnitTestFixture1.htm:testFailingTest1|0|E|\"This test should error\",http://localhost/JsUnitTestFixture2.htm:testFailingTest2|7|F|\"This test should also fail\"";

            var results = parser.ParseJsUnitErrors(errorResponse);
            Assert.AreEqual(2, results.Count);

            Assert.AreEqual("http://localhost:80/JsUnitTestFixture1.htm", results[0].TestPage);
            Assert.AreEqual("testFailingTest1", results[0].FunctionName);
            Assert.AreEqual("\"This test should error\"", results[0].Message);
            Assert.AreEqual("0", results[0].Timing);
            Assert.AreEqual(TestResult.Error, results[0].TestResult);
            Assert.IsTrue(results[0].IsError, "IsError should be true");

            Assert.AreEqual("http://localhost/JsUnitTestFixture2.htm", results[1].TestPage);
            Assert.AreEqual("testFailingTest2", results[1].FunctionName);
            Assert.AreEqual("\"This test should also fail\"", results[1].Message);
            Assert.AreEqual("7", results[1].Timing);
            Assert.AreEqual(TestResult.Failure, results[1].TestResult);
            Assert.IsTrue(results[1].IsError, "IsError should be true");
        }

        [Test]
        public void Should_return_single_success()
        {
            const string errorResponse =
                "http://localhost/JsUnitTestFixture1.htm:testPassingTest1|0|S||";

            var results = parser.ParseJsUnitErrors(errorResponse);
            Assert.AreEqual(1, results.Count);

            Assert.AreEqual("http://localhost/JsUnitTestFixture1.htm", results[0].TestPage);
            Assert.AreEqual("testPassingTest1", results[0].FunctionName);
            Assert.AreEqual("", results[0].Message);
            Assert.AreEqual("0", results[0].Timing);
            Assert.AreEqual(TestResult.Success, results[0].TestResult);
            Assert.IsFalse(results[0].IsError, "IsError should be false");
        }
    }
}
