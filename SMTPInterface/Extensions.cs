public static class Extensions
{
    public static string? EmptyOrNull(this string? source) => source == null || source.Length == 0 ? null : source;
    public static string[]? TrySplit(this string? source, char separator = ' ') => source == null ? null : source.Split(separator);
}