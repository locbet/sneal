using System;

namespace Sneal.Preconditions.Aop
{
    /// <summary>
    /// Attribute can be added to a method argument to ensure that the value
    /// passed at runtime is not null.
    /// </summary>
    /// <remarks>
    /// The ReSharper NotNull or a custom attribute can also be used instead of this.
    /// </remarks>
    [AttributeUsageAttribute(
        AttributeTargets.Parameter | AttributeTargets.Property,
        Inherited = true, AllowMultiple = false)]
    public class NotNullAttribute : Attribute
    {

    }

    /// <summary>
    /// Attribute can be added to a string method argument to ensure that the value
    /// passed at runtime is not null or empty.
    /// </summary>
    /// <remarks>
    /// The ReSharper NotNullOrEmpty or a custom attribute can also be used instead of this.
    /// </remarks>    
    [AttributeUsageAttribute(
        AttributeTargets.Parameter | AttributeTargets.Property,
        Inherited = true, AllowMultiple = false)]
    public class NotNullOrEmptyAttribute : Attribute
    {

    }

}
