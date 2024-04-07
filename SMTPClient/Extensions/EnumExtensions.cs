using EmailClient.Common;

namespace EmailClient.Extensions;

internal static class EnumExtensions
{
    public static bool InValidCodes(this SMTPResponseCode code, params SMTPResponseCode[] validCodes)
    {
        return validCodes.Contains(code);
    }
}