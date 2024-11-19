using System.Runtime.CompilerServices;
using Employee.RpcService.Exceptions;

namespace Employee.RpcService.Helpers;

public static class ValidationHelpers
{
    /// <summary>
    /// Ensure that T isn't null or throw the <see cref="InvalidArgumentEmployeeException"/>.
    /// </summary>
    public static T EnsureNotNull<T>(
        this T? value,
        [CallerArgumentExpression("value")] string propertyName = "")
    {
        if (value is not null)
            return value;

        throw new InvalidArgumentEmployeeException(
            string.Format(
                ErrorMessages.ShouldBeNotNull,
                propertyName.GetPurePropertyName()));
    }
    
    private static string? GetPurePropertyName(this string? propertyName)
    {
        const char dot = '.';
        
        if (string.IsNullOrEmpty(propertyName))
        {
            return propertyName;
        }

        var dotPosition = propertyName.LastIndexOf(dot);
        
        return dotPosition == -1 ? propertyName : propertyName[(dotPosition + 1) ..];
    }

}