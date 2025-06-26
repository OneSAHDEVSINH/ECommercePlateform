using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Common.Validation
{
    public static partial class GeneratedRegex
    {
        public static Regex CapitalizedWords()
        {
            return CapitalizedWordsRegex();
        }

        [GeneratedRegex(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$", RegexOptions.Compiled)]
        private static partial Regex CapitalizedWordsRegex();

        public static Regex UppercaseLetters()
        {
            return UppercaseLettersRegex();
        }

        [GeneratedRegex(@"^[A-Z]{1,3}$", RegexOptions.Compiled)]
        private static partial Regex UppercaseLettersRegex();

        public static Regex Email()
        {
            return EmailRegex();
        }

        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
        private static partial Regex EmailRegex();

        public static Regex Name()
        {
            // Pattern allows letters from any language, spaces, hyphens, and apostrophes
            return NameRegex();
        }

        [GeneratedRegex(@"^[\p{L}\s'-]+$", RegexOptions.Compiled)]
        private static partial Regex NameRegex();

        public static Regex AlphanumericWithSpaces()
        {
            return AlphanumericWithSpacesRegex();
        }

        [GeneratedRegex(@"^[a-zA-Z0-9 ]+$", RegexOptions.Compiled)]
        private static partial Regex AlphanumericWithSpacesRegex();

        public static Regex RouteFormat()
        {
            return RouteFormatRegex();
        }

        [GeneratedRegex("^[a-z0-9-]+$", RegexOptions.Compiled)]
        private static partial Regex RouteFormatRegex();
    }
}