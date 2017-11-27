namespace ITOps.Json
{
    internal static class StringExtensions
    {
        public static string ToPascalCase(this string text)
        {
            char initial;
            if (string.IsNullOrEmpty(text) || !char.IsLower(initial = text[0]))
            {
                return text;
            }

            var upperInitial = char.ToUpperInvariant(initial).ToString();

            return text.Length == 1
                ? upperInitial
                : upperInitial + text.Substring(1);
        }
    }
}
