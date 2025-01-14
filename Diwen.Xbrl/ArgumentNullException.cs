#if NETSTANDARD2_0

using System.Runtime.CompilerServices;

namespace Diwen.Compat;

/// <summary>
/// Polyfill for the ArgumentNullException factory method from net6.0+
/// </summary>
public static class ArgumentNullException
{
    /// <summary>Throws an <see cref="T:System.ArgumentNullException" /> if <paramref name="argument" /> is <see langword="null" />.</summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which <paramref name="argument" /> corresponds.</param>
    /// <exception cref="T:System.ArgumentNullException">
    /// <paramref name="argument" /> is <see langword="null" />.</exception>
    public static void ThrowIfNull(object argument, [CallerArgumentExpression("argument")] string paramName = null)
    {
        if (argument is null)
        {
            throw new System.ArgumentNullException(paramName);
        }
    }
}
#endif