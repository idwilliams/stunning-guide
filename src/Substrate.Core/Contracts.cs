using System.Diagnostics;

namespace Substrate.Core;

/// <summary>
/// Debug-only contract validation helpers.
/// </summary>
public static class Contracts
{
    [Conditional("DEBUG")]
    public static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException($"Contract violation: {message}");
        }
    }

    [Conditional("DEBUG")]
    public static void RequireNotNull<T>(T? value, string paramName) where T : class
    {
        if (value == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
