using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Commons.Helpers;

public sealed class EnumHelpers
{
    public static string Convert<T>(T enumValue)
    {
        var result = Enum.GetName(typeof(T), enumValue);
        return result;
    }
    
    public static T ToEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }
}