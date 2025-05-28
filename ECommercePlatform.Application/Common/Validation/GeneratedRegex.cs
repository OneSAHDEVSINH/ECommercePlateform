using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        [GeneratedRegex(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$", RegexOptions.Compiled)]
        private static partial Regex CapitalizedWordsRegex();
        [GeneratedRegex(@"^[A-Z]{1,3}$", RegexOptions.Compiled)]
        private static partial Regex UppercaseLettersRegex();
    }
}
