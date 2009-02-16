using System;

namespace Sneal.JsUnitUtils.MsBuild
{
    /// <summary>
    /// Formats a <see cref="JsUnitErrorResult"/> for use in MSBuild output.
    /// </summary>
    public class JsUnitErrorFormatProvider : IFormatProvider, ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var result = arg as JsUnitErrorResult;
            if (result == null)
            {
                return arg.ToString();
            }

            if (result.IsError)
            {
                return string.Format(
                    "{0}: {1} had a JavaScript error occur during execution",
                    result.TestPage, result.FunctionName);
            }

            if (string.IsNullOrEmpty(result.Message))
            {
                return string.Format(
                    "{0}: {1} failed without a message",
                    result.TestPage, result.FunctionName);
            }

            return string.Format(
                "{0}: {1} failed with message: {2}",
                result.TestPage, result.FunctionName, result.Message);            
        }

        public object GetFormat(Type formatType)
        {
            return this;
        }
    }
}
