using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Common.Validation
{
    public static class GeneratedRegex
    {
        public static Regex CapitalizedWords()
        {
            return new Regex(@"^([A-Z][a-z]*)(?: [A-Z][a-z]*)*$", RegexOptions.Compiled);
        }

        public static Regex UppercaseLetters()
        {
            return new Regex(@"^[A-Z]{1,3}$", RegexOptions.Compiled);
        }
    }
}
