using System.Text.RegularExpressions;

namespace ECommercePlatform.Server.src.Core.Application.Services
{
    public class ValidationService
    {
        public static bool IsValidNameorCode(string name, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Name or Code cannot be empty or contain only whitespace.";
                return false;
            }

            if (!Regex.IsMatch(name.Trim(), @"^[A-Za-z\s]+$"))
            {
                errorMessage = "Name or Code can only contain letters and spaces.";
                return false;
            }

            return true;
        }

        public static bool IsValidNameorCode(string code, string name, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                errorMessage = "Name or Code cannot be empty or contain only whitespace.";
                return false;
            }

            if (!Regex.IsMatch(name.Trim(), @"^[A-Za-z\s]+$"))
            {
                errorMessage = "Name or Code can only contain letters and spaces.";
                return false;
            }

            return true;
        }

    }
}
