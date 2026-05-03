using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Mentor
{
    public class StringHelpers
    {
        public static string SanitizeFileNamePascalCase(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "NewAsset"; // Or handle as you prefer

            // 1. Remove invalid path characters
            string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            string validName = Regex.Replace(name, $"[{invalidChars}]+", ""); // Remove invalid chars, don't replace with _

            // 2. Process spaces to create PascalCase
            StringBuilder pascalCaseName = new StringBuilder();
            bool capitalizeNext = true; // Start with true to capitalize the first character

            foreach (char c in validName)
            {
                if (char.IsWhiteSpace(c))
                    capitalizeNext = true; // The next non-space character should be capitalized
                else
                {
                    if (capitalizeNext)
                    {
                        pascalCaseName.Append(char.ToUpper(c));
                        capitalizeNext = false; // Reset for subsequent characters
                    }
                    else
                    {
                        pascalCaseName.Append(char.ToLower(c)); // Keep subsequent chars lowercase
                    }
                }
            }

            // 3. Convert to string and trim any leading/trailing whitespace that might have slipped through
            // (though the logic above should prevent leading/trailing spaces from impacting the final string content)
            string finalName = pascalCaseName.ToString().Trim();


            // 4. Optional: Ensure the filename is not empty after sanitization
            if (string.IsNullOrWhiteSpace(finalName))
            {
                finalName = "default_filename";
            }

            // 5. Optional: Truncate if the filename is too long (max length is 255 characters for most file systems)
            if (finalName.Length > 255)
            {
                finalName = finalName.Substring(0, 255);
                // Ensure it doesn't end with an unwanted character after truncation (though unlikely with this logic)
            }

            return finalName;
        }
    }
}