namespace ITOps.ViewModelComposition.Json
{
    public static class StringExtensions
    {
        public static string ToPascalCase(this string s)
        {
            if(string.IsNullOrEmpty(s) || !char.IsLower(s[ 0 ]))
                return s;

            string str = char.ToUpperInvariant(s[ 0 ]).ToString();

            if(s.Length > 1)
                str = str + s.Substring(1);

            return str;
        }
    }
}
