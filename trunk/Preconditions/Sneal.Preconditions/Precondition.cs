using System;

namespace Sneal.Preconditions
{
    public class Precondition<TArgument>
    {
        protected TArgument argument;
        protected string argumentName;

        public Precondition(TArgument argument, string argumentName)
        {
            this.argument = argument;
            this.argumentName = argumentName;
        }

        public void IsNull()
        {
            if (argument == null)
            {
                throw new ArgumentNullException(
                    argumentName,
                    string.Format("The argument {0} cannot be null", argumentName));
            }
        }

        public void IsEqualTo(TArgument equalTo)
        {
            if (equalTo.Equals(argument))
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be equal to {1}", argumentName, equalTo));
            }
        }

        public void IsLessThan(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == 1)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be less than {1}", argumentName, comparer));
            }
        }

        public void IsGreaterThan(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == -1)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be greater than {1}", argumentName, comparer));
            }
        }

        public void IsComparableTo(IComparable<TArgument> comparer)
        {
            if (comparer.CompareTo(argument) == 0)
            {
                throw new ArgumentException(
                    argumentName,
                    string.Format("The argument {0} cannot be comparable to {1}", argumentName, comparer));
            }
        }
    }
}