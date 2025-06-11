using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Common.Validation
{
    public static partial class GeneratedRegex
    {
        public static Regex CapitalizedWords()
        {
            return CapitalizedWordsRegex();
        }

        public static Regex UppercaseLetters()
        {
            return UppercaseLettersRegex();
        }

        public static Regex Email()
        {
            return new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        }

        public static Regex Name()
        {
            // Pattern allows letters from any language, spaces, hyphens, and apostrophes
            return new Regex(@"^[\p{L}\s'-]+$", RegexOptions.Compiled);
        }

        public static Regex AlphanumericWithSpaces()
        {
            return new Regex(@"^[a-zA-Z0-9 ]+$", RegexOptions.Compiled);
        }

        public static Regex RouteFormat()
        {
            return new Regex("^[a-z0-9-]+$", RegexOptions.Compiled);
        }

        [GeneratedRegex(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$", RegexOptions.Compiled)]
        private static partial Regex CapitalizedWordsRegex();

        [GeneratedRegex(@"^[A-Z]{1,3}$", RegexOptions.Compiled)]
        private static partial Regex UppercaseLettersRegex();
    }
}