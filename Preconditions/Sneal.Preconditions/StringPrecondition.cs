using System;
using System.Text.RegularExpressions;

namespace Sneal.Preconditions
{
    public class StringPrecondition : Precondition<string>
    {
        public StringPrecondition(string argument, string argumentName)
            : base(argument, argumentName)
        {
        }

        public void IsEmpty()
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} cannot be empty", argumentName));
            }
        }

        public void Matches(string regexExpression)
        {
            Throw.If(regexExpression, "regexExpression").IsEmpty();

            Regex regex = new Regex(regexExpression);
            if (regex.Match(argument).Success)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} cannot match the regular expression {1}", argumentName,
                                  regexExpression));
            }
        }

        public void DoesNotMatch(string regexExpression)
        {
            Throw.If(regexExpression, "regexExpression").IsEmpty();

            Regex regex = new Regex(regexExpression);
            if (!regex.Match(argument).Success)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} must match the regular expression {1}", argumentName,
                                  regexExpression));
            }
        }

        public void LengthIsLessThan(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length < length)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} length must not be less than {1}", argumentName, length));
            }
        }

        public void LengthIsLessThanOrEqualTo(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length <= length)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} length must not be less than or equal to {1}", argumentName,
                                  length));
            }
        }

        public void LengthIsGreaterThan(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length > length)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} length must not be greater than {1}", argumentName, length));
            }
        }

        public void LengthIsGreaterThanOrEqualTo(int length)
        {
            Throw.If(argument, argumentName).IsNull();

            if (argument.Length >= length)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The string argument {0} length must not be greater than or equal to {1}",
                                  argumentName, length));
            }
        }
    }
}