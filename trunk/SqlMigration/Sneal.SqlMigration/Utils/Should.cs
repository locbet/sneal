using System;

namespace Sneal.SqlMigration.Utils
{
    /// <summary>
    /// Utility for testing method parameters.
    /// </summary>
    internal static class Should
    {
        internal static void NotBeNull(object param)
        {
            if (param == null)
                throw new ArgumentNullException("param", "The parameter should not be null.");
        }

        internal static void NotBeNull(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName, "The parameter should not be null.");
        }

        internal static void NotBeNullOrEmpty(string param)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException("param", "The parameter should not be null.");
        }

        internal static void NotBeNullOrEmpty(string param, string paramName)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentNullException(paramName, "The parameter should not be null.");
        }
    }
}