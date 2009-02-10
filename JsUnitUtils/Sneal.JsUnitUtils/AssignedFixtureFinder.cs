using Sneal.Preconditions.Aop;

namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Allows the testRunner.html location to be explicitly specified.
    /// </summary>
    public class AssignedFixtureFinder : IFixtureFinder
    {
        private readonly IWebServer webServer;
        private readonly string fullyQualifiedLocalPath;

        /// <summary>
        /// Creates a new JS Unit fixture finder instance.
        /// </summary>
        /// <param name="webServer">The web server instance to search under.</param>
        /// <param name="fullyQualifiedLocalPath">
        /// The fully qualified local path to the JsUnit testRunner.html file.
        /// </param>
        public AssignedFixtureFinder([NotNull]IWebServer webServer, [NotNullOrEmpty] string fullyQualifiedLocalPath)
        {
            this.webServer = webServer;
            this.fullyQualifiedLocalPath = fullyQualifiedLocalPath;
        }

        public string GetTestRunnerPath()
        {
            return webServer.MakeHttpUrl(fullyQualifiedLocalPath);
        }
    }
}
