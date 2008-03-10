namespace Sneal.Preconditions
{
    /// <summary>
    /// Static factory for generating argument preconditions.
    /// </summary>
    public static class Throw
    {
        public static Precondition<TArgument> If<TArgument>(TArgument argument, string argumentName)
        {
            return new Precondition<TArgument>(argument, argumentName);
        }

        public static Precondition<TArgument> If<TArgument>(TArgument argument)
        {
            return new Precondition<TArgument>(argument, null);
        }

        public static StringPrecondition If(string argument, string argumentName)
        {
            return new StringPrecondition(argument, argumentName);
        }

        public static StringPrecondition If(string argument)
        {
            return new StringPrecondition(argument, null);
        }
    }
}