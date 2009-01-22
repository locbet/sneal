using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace Sneal.ReSharper.MsTest
{
    internal class MSTestTaskRunner : RemoteTaskRunner
    {
        private static MethodInfo myFixtureSetUp;
        private static MethodInfo myFixtureTearDown;
        private static MethodInfo myTestSetUp;
        private static MethodInfo myTestTearDown;

        public MSTestTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override TaskResult Start(TaskExecutionNode node)
        {
            RemoteTask task = node.RemoteTask;
            if (task is MSTestFixtureTask)
                return Start(Server, node, (MSTestFixtureTask) task);
            if (task is MSTestTask)
                return Start(Server, node, (MSTestTask) task);
            return TaskResult.Error;
        }

        public override TaskResult Execute(TaskExecutionNode node)
        {
            RemoteTask task = node.RemoteTask;
            if (task is MSTestFixtureTask)
                return TaskResult.Success;
            if (task is MSTestTask)
                return Execute(Server, node, (MSTestTask) task);
            return TaskResult.Error;
        }

        public override TaskResult Finish(TaskExecutionNode node)
        {
            RemoteTask task = node.RemoteTask;
            if (task is MSTestFixtureTask)
            {
                return Finish(Server, (MSTestFixtureTask) task);
            }
            if (task is MSTestTask)
            {
                return Finish(Server, node, (MSTestTask) task);
            }
            return TaskResult.Error;
        }

        public override void ConfigureAppDomain(TaskAppDomainConfiguration configuration)
        {
            var settings = ConfigurationManager.GetSection("MSTest/TestRunner") as NameValueCollection;
            if (settings != null)
            {
                string apartment = settings["ApartmentState"];
                if (apartment != null)
                    try
                    {
                        configuration.ApartmentState =
                            (ApartmentState) Enum.Parse(typeof (ApartmentState), apartment, true);
                    }
                    catch (ArgumentException ex)
                    {
                        string msg = string.Format("Invalid ApartmentState setting '{1}' in configuration file '{0}'",
                                                   AppDomain.CurrentDomain.SetupInformation.ConfigurationFile, apartment);
                        throw new ArgumentException(msg, ex);
                    }

                string threadPriority = settings["ThreadPriority"];
                if (threadPriority != null)
                    try
                    {
                        configuration.Priority =
                            (ThreadPriority) Enum.Parse(typeof (ThreadPriority), threadPriority, true);
                    }
                    catch (ArgumentException ex)
                    {
                        string msg = string.Format("Invalid ThreadPriority setting '{1}' in '{0}'",
                                                   AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                                                   threadPriority);
                        throw new ArgumentException(msg, ex);
                    }
            }
        }

        private static Type GetFixtureType(MSTestFixtureTask fixture, IRemoteTaskServer server)
        {
            string assemblyLocation = fixture.AssemblyLocation;
            if (!File.Exists(assemblyLocation))
            {
                server.TaskError(fixture,
                                 string.Format("Cannot load assembly from {0}: file not exists", assemblyLocation));
                return null;
            }
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(assemblyLocation);
            if (assemblyName == null)
            {
                server.TaskError(fixture,
                                 string.Format("Cannot load assembly from {0}: not an assembly", assemblyLocation));
                return null;
            }
            Assembly assembly = Assembly.Load(assemblyName);
            if (assembly == null)
            {
                server.TaskError(fixture, string.Format("Cannot load assembly from {0}", assemblyLocation));
                return null;
            }
            return assembly.GetType(fixture.TypeName);
        }

        private static bool BuildTypeInfo(IRemoteTaskServer server, RemoteTask fixture, Type type)
        {
            myTestTearDown = null;
            myTestSetUp = null;
            myFixtureSetUp = null;
            myFixtureTearDown = null;

            IList<MethodInfo> fixtureSetUp = new List<MethodInfo>();
            IList<MethodInfo> fixtureTearDown = new List<MethodInfo>();
            IList<MethodInfo> testSetUp = new List<MethodInfo>();
            IList<MethodInfo> testTearDown = new List<MethodInfo>();

            MethodInfo[] methods =
                type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                                BindingFlags.NonPublic);
            foreach (MethodInfo method in methods)
            {
                if (IsFixtureSetup(method)) fixtureSetUp.Add(method);
                else if (IsFixtureTeardown(method)) fixtureTearDown.Add(method);
                else if (IsTestSetup(method)) testSetUp.Add(method);
                else if (IsTestTeardown(method)) testTearDown.Add(method);
            }

            fixtureSetUp = FilterHiddenMethods(fixtureSetUp);
            if (fixtureSetUp.Count == 1)
                myFixtureSetUp = fixtureSetUp[0];
            else if (fixtureSetUp.Count > 1)
            {
                server.TaskError(fixture, type.Name + " has multiple fixture setup methods");
                return false;
            }

            fixtureTearDown = FilterHiddenMethods(fixtureTearDown);
            if (fixtureTearDown.Count == 1)
                myFixtureTearDown = fixtureTearDown[0];
            else if (fixtureTearDown.Count > 1)
            {
                server.TaskError(fixture, type.Name + " has multiple fixture teardown methods");
                return false;
            }

            testSetUp = FilterHiddenMethods(testSetUp);
            if (testSetUp.Count == 1)
                myTestSetUp = testSetUp[0];
            else if (testSetUp.Count > 1)
            {
                server.TaskError(fixture, type.Name + " has multiple test setup methods");
                return false;
            }

            testTearDown = FilterHiddenMethods(testTearDown);
            if (testTearDown.Count == 1)
                myTestTearDown = testTearDown[0];
            else if (testTearDown.Count > 1)
            {
                server.TaskError(fixture, type.Name + " has multiple test teardown methods");
                return false;
            }
            return true;
        }

        private static IList<MethodInfo> FilterHiddenMethods(IList<MethodInfo> methods)
        {
            if (methods.Count <= 1)
                return methods;

            var newMethods = new List<MethodInfo>();

            foreach (MethodInfo info in methods)
            {
                Type declaringType = info.DeclaringType;

                bool isvisible = true;
                foreach (MethodInfo info2 in methods)
                {
                    Type declaringType2 = info2.DeclaringType;
                    if (declaringType != declaringType2 && !declaringType.IsSubclassOf(declaringType2))
                    {
                        isvisible = false;
                        break;
                    }
                }

                if (isvisible)
                    newMethods.Add(info);
            }

            return newMethods;
        }

        private static bool IsFixtureSetup(MethodInfo info)
        {
            return CheckSetUpTearDownSignature(info,
                "Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute");
        }

        private static bool IsFixtureTeardown(MethodInfo info)
        {
            return CheckSetUpTearDownSignature(info,
                "Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute");
        }

        private static bool IsTestSetup(MethodInfo info)
        {
            return CheckSetUpTearDownSignature(info,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitialize");
        }

        private static bool IsTestTeardown(MethodInfo info)
        {
            return CheckSetUpTearDownSignature(info,
                "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanup");
        }

        private static bool IsSubtypeOf(Type type, string name)
        {
            while (type != null)
            {
                if (type.FullName == name)
                    return true;
                type = type.BaseType;
            }
            return false;
        }

        private static bool IsSubtypeOf(object obj, string name)
        {
            return IsSubtypeOf(obj.GetType(), name);
        }

        private static object[] GetCustomAttributesOf(ICustomAttributeProvider mi, bool inherit, string typeName)
        {
            return SelectAttributes(mi.GetCustomAttributes(inherit), typeName);
        }

        private static object[] SelectAttributes(object[] attrs, string typeName)
        {
            var found = new List<object>();
            foreach (object attr in attrs)
            {
                if (IsSubtypeOf(attr, typeName))
                    found.Add(attr);
            }

            return found.ToArray();
        }

        private static bool CheckSetUpTearDownSignature(MethodInfo method, string attrName)
        {
            object[] attributes = GetCustomAttributesOf(method, true, attrName);
            if (attributes.Length == 0)
                return false;

            int expectedNumParams = attrName.Contains("ClassInitializeAttribute") ? 1 : 0;

            if (!method.IsPublic && !method.IsFamily || method.ReturnType != typeof (void) ||
                method.GetParameters().Length != expectedNumParams)
                return false;

            return true;
        }

        private static object GetPropertyValue(object obj, string propName)
        {
            MemberInfo[] members = obj.GetType().GetMember(propName);
            if (members.Length != 1)
                return null;

            var propertyInfo = members[0] as PropertyInfo;
            if (propertyInfo == null)
                return null;

            return propertyInfo.GetValue(obj, null);
        }

        private static void GetExpectedException(ICustomAttributeProvider info, out string expectedExceptionType)
        {
            expectedExceptionType = null;

            object[] attributes = GetCustomAttributesOf(info, false,
                "Microsoft.VisualStudio.TestTools.UnitTesting.ExpectedExceptionAttribute");
            if (attributes.Length == 1)
            {
                object exceptionType = GetPropertyValue(attributes[0], "ExceptionType");
                if (exceptionType != null)
                    expectedExceptionType = ((Type) exceptionType).FullName;
            }
        }

        private static bool IsIgnored(ICustomAttributeProvider info)
        {
            object[] attributes = GetCustomAttributesOf(info, false,
                "Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute");
            return (attributes.Length > 0);
        }

        #region Test

        private TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node, MSTestTask test)
        {
            var fixture = (MSTestFixtureTask) node.Parent.RemoteTask;

            object instance = fixture.Instance;
            Type type = instance.GetType();

            MethodInfo testMI = type.GetMethod(test.TestMethod, new Type[0]);
            if (testMI == null)
            {
                server.TaskError(test, string.Format("Cannot find test  method '{0}'", test.TestMethod));
                return TaskResult.Error;
            }

            if (IsIgnored(testMI) && !test.Explicitly)
            {
                server.TaskFinished(test, null, TaskResult.Skipped);
                return TaskResult.Skipped;
            }

            if (myTestSetUp != null)
            {
                server.TaskProgress(test, "Setting up...");
                try
                {
                    TaskExecutor.Invoke(fixture.Instance, myTestSetUp);
                }
                catch (TargetInvocationException e)
                {
                    Exception exception = e.InnerException ?? e;

                    string message;
                    Server.TaskException(test, TaskExecutor.ConvertExceptions(exception, out message));
                    Server.TaskFinished(test, message, TaskResult.Exception);
                    return TaskResult.Exception;
                }
            }

            return TaskResult.Success;
        }


        private static TaskResult Execute(IRemoteTaskServer server, TaskExecutionNode node, MSTestTask test)
        {
            var fixture = (MSTestFixtureTask) node.Parent.RemoteTask;
            object instance = fixture.Instance;
            Type type = instance.GetType();

            MethodInfo testMI = type.GetMethod(test.TestMethod, new Type[0]);
            if (testMI == null)
            {
                server.TaskError(test, string.Format("Cannot find test  method '{0}'", test.TestMethod));
                return TaskResult.Error;
            }
            server.TaskProgress(test, "");

            string expectedExceptionType;
            GetExpectedException(testMI, out expectedExceptionType);

            Exception exception = null;
            try
            {
                TaskExecutor.Invoke(instance, testMI);
            }
            catch (TargetInvocationException e)
            {
                exception = e.InnerException ?? e;
            }
            if (exception != null && exception.GetType().FullName == "csUnit.IgnoreException")
            {
                server.TaskFinished(test, exception.Message, TaskResult.Skipped);
                return TaskResult.Skipped;
            }
            if (expectedExceptionType != null && exception == null)
            {
                // failed, exception expected but not thrown
                server.TaskError(test, string.Format("Expected exception '{0}' was not thrown", expectedExceptionType));
                return TaskResult.Error;
            }
            if (expectedExceptionType != null && expectedExceptionType == exception.GetType().FullName)
            {
                return TaskResult.Success;
            }
            if (exception != null)
                throw new TargetInvocationException(exception);

            return TaskResult.Success;
        }

        private TaskResult Finish(IRemoteTaskServer server, TaskExecutionNode node, MSTestTask test)
        {
            var fixture = (MSTestFixtureTask) node.Parent.RemoteTask;

            if (myTestTearDown != null)
            {
                server.TaskProgress(test, "Tearing down...");
                try
                {
                    TaskExecutor.Invoke(fixture.Instance, myTestTearDown);
                }
                catch (TargetInvocationException e)
                {
                    Exception exception = e.InnerException ?? e;

                    string message;
                    Server.TaskException(test, TaskExecutor.ConvertExceptions(exception, out message));
                    Server.TaskFinished(test, message, TaskResult.Exception);
                    return TaskResult.Exception;
                }
            }
            server.TaskProgress(test, "");
            return TaskResult.Success;
        }

        #endregion

        #region Fixture

        private TaskResult Start(IRemoteTaskServer server, TaskExecutionNode node, MSTestFixtureTask fixture)
        {
            server.TaskProgress(fixture, "Instantiating...");
            Type type = GetFixtureType(fixture, server);
            if (type == null)
                return TaskResult.Error;

            if (IsIgnored(type))
            {
                // fixture is ignored
                if (!fixture.Explicitly)
                {
                    // check that we don't have any explicitly run test
                    bool hasExplicitTest = false;
                    foreach (TaskExecutionNode testNode in node.Children)
                    {
                        if (((MSTestTask) testNode.RemoteTask).Explicitly)
                        {
                            hasExplicitTest = true;
                            break;
                        }
                    }
                    if (!hasExplicitTest)
                    {
                        server.TaskProgress(fixture, "");
                        server.TaskFinished(fixture, null, TaskResult.Skipped);
                        return TaskResult.Skipped;
                    }
                }
            }

            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
            if (ci == null)
            {
                server.TaskError(fixture, string.Format("Cannot find parameterless constructor on type '{0}'", type));
                return TaskResult.Error;
            }

            if (!BuildTypeInfo(server, fixture, type))
                return TaskResult.Error;

            object instance = ci.Invoke(new object[0]);
            if (instance == null)
            {
                server.TaskError(fixture, string.Format("Cannot create instance of type '{0}'", type));
                return TaskResult.Error;
            }

            fixture.Instance = instance;
            if (myFixtureSetUp != null)
            {
                server.TaskProgress(fixture, "Setting up...");
                try
                {
                    TaskExecutor.Invoke(fixture.Instance, myFixtureSetUp);
                }
                catch (TargetInvocationException e)
                {
                    Exception exception = e.InnerException ?? e;

                    string message;
                    Server.TaskException(fixture, TaskExecutor.ConvertExceptions(exception, out message));
                    Server.TaskFinished(fixture, message, TaskResult.Exception);
                    return TaskResult.Exception;
                }
            }

            server.TaskProgress(fixture, "");
            return TaskResult.Success;
        }

        private TaskResult Finish(IRemoteTaskServer server, MSTestFixtureTask fixture)
        {
            if (myFixtureTearDown != null)
            {
                server.TaskProgress(fixture, "Tearing down...");
                try
                {
                    TaskExecutor.Invoke(fixture.Instance, myFixtureTearDown);
                }
                catch (TargetInvocationException e)
                {
                    Exception exception = e.InnerException ?? e;

                    string message;
                    Server.TaskException(fixture, TaskExecutor.ConvertExceptions(exception, out message));
                    Server.TaskFinished(fixture, message, TaskResult.Exception);
                    return TaskResult.Exception;
                }
            }

            server.TaskProgress(fixture, "");
            myTestTearDown = null;
            myTestSetUp = null;
            myFixtureSetUp = null;
            myFixtureTearDown = null;
            fixture.Instance = null;
            return TaskResult.Success;
        }

        #endregion
    }
}