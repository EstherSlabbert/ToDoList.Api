using System.Runtime.CompilerServices;

namespace ToDoList.Common;

public static class ArgumentChecks
{
    public static void ThrowIfNullOrWhiteSpace(this string @this, [CallerArgumentExpression(nameof(@this))] string parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(parameterName))
            throw new InvalidParameterException($@"The argument ""{nameof(parameterName)}"" was not provided.");

        if (string.IsNullOrWhiteSpace(@this))
        {
            var propertyName = GetPropertyName(parameterName);
            throw new InvalidParameterException($@"The argument ""{propertyName}"" was not provided.");
        }
    }
    public static void ThrowIfNull<T>(this T @this, [CallerArgumentExpression(nameof(@this))] string parameterName = null)
    {
        parameterName.ThrowIfNullOrWhiteSpace();

        var propertyName = GetPropertyName(parameterName);
        if (Equals(@this, default(T)))
            throw new InvalidParameterException($@"The argument ""{propertyName}"" was not provided.");
    }

    private static string GetPropertyName(string parameterName)
    {
        var parts = parameterName.Split('.');
        return parts[^1];
    }

    public static void ThrowIf<T>(this T @this, Func<T, bool> condition, [CallerArgumentExpression(nameof(@this))] string parameterName = null, string message = null)
    {
        parameterName.ThrowIfNullOrWhiteSpace();
        if (condition(@this))
            throw new ArgumentOutOfRangeException(parameterName, message ?? $@"The argument ""{parameterName}"" is invalid.");
    }
}
