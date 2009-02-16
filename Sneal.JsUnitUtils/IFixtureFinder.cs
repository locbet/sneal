namespace Sneal.JsUnitUtils
{
    /// <summary>
    /// Locates the JsUnit test runner html file within the specified
    /// webserver path.
    /// </summary>
    public interface IFixtureFinder
    {
        /// <summary>
        /// Locates the testRunner.html file underneath the given webserver and
        /// returns the HTTP path to the file.
        /// </summary>
        /// <remarks>
        /// If found, the path is cached within this instance.
        /// </remarks>
        /// <returns>The full HTTP path to the fixture runner html file.</returns>
        string GetTestRunnerPath();
    }
}