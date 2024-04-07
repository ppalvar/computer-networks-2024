using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailClient.Extensions;

internal static class StringExtensions
{
    public static bool IsNumeric(this string src)
    {
        var rgex = new Regex("[0-9]*");
        return rgex.IsMatch(src);
    }

    public static string ToBase64(this string source) => Convert.ToBase64String(Encoding.ASCII.GetBytes(source));
    public static string ToBase16(this string source) => Convert.ToHexString(Encoding.ASCII.GetBytes(source));
    public static string FromBase64(this string source) => Encoding.ASCII.GetString(Convert.FromBase64String(source));
    public static string ToMd5Hash(this string source, string key)
    {
        using (var md5 = new HMACMD5(Encoding.ASCII.GetBytes(key)))
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(source);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hashBytes.Length; i++)
            {
                sb.Append(hashBytes[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }

    public static string Summarize(this string source, int maxLenght = 200)
    {
        if (maxLenght >= source.Length) return source;
        return source[0..maxLenght] + "<...>";
    }

    public static string Merge(this IEnumerable<string> source)
    {
        var builder = new StringBuilder();

        foreach (var s in source) builder.Append($"{s}, ");

        return builder.ToString();
    }
}