using System.Text.RegularExpressions;

namespace ECommercePlatform.Application.Services
{
    public partial class ValidationService
    {
        public static bool IsValidNameorCode(string name, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Name or Code cannot be empty or contain only whitespace.";
                return false;
            }

            if (!NameOrCodeRegex().IsMatch(name.Trim()))
            {
                errorMessage = "Name or Code can only contain letters and spaces.";
                return false;
            }

            return true;
        }

        public static bool IsValidNameorCode(string code, string name, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(code))
            {
                errorMessage = "Name or Code cannot be empty or contain only whitespace.";
                return false;
            }

            if (!NameOrCodeValidatorRegex().IsMatch(name.Trim()))
            {
                errorMessage = "Name or Code can only contain letters and spaces.";
                return false;
            }

            return true;
        }

        [GeneratedRegex(@"^[A-Za-z\s]+$")]
        private static partial Regex NameOrCodeRegex();
        [GeneratedRegex(@"^[A-Za-z\s]+$")]
        private static partial Regex NameOrCodeValidatorRegex();
    }
}
