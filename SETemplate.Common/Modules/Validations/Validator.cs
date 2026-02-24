//@BaseCode
namespace SETemplate.Common.Modules.Validations
{
    using SETemplate.Common.Modules.Security;
    using System.Text.RegularExpressions;

    public static partial class Validator
    {
        /// <summary>
        /// Checks if the provided email is valid.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <returns>True if the email is valid, otherwise false.</returns>
        public static bool CheckEmailSyntax(string email)
        {
            var result = false;

            if (string.IsNullOrWhiteSpace(email) == false)
            {
                string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

                result = Regex.IsMatch(email, pattern);
            }
            return result;
        }

        /// <summary>
        /// Checks if the password matches the password rules.
        /// </summary>
        /// <param name="password">The password to check</param>
        /// <returns>True if the password matches Password Rules, false otherwise</returns>
        public static bool CheckPasswordSyntax(string password)
        {
            var digitCount = 0;
            var letterCount = 0;
            var lowerLetterCount = 0;
            var upperLetterCount = 0;
            var specialLetterCount = 0;

            foreach (char ch in password)
            {
                if (char.IsDigit(ch))
                {
                    digitCount++;
                }
                else
                {
                    if (char.IsLetter(ch))
                    {
                        letterCount++;
                        if (char.IsLower(ch))
                        {
                            lowerLetterCount++;
                        }
                        else
                        {
                            upperLetterCount++;
                        }
                    }
                    else
                    {
                        specialLetterCount++;
                    }
                }
            }
            return password.Length >= PasswordRules.MinimumLength
                  && password.Length <= PasswordRules.MaximumLength
                  && letterCount >= PasswordRules.MinLetterCount
                  && upperLetterCount >= PasswordRules.MinUpperLetterCount
                  && lowerLetterCount >= PasswordRules.MinLowerLetterCount
                  && specialLetterCount >= PasswordRules.MinSpecialCharCount
                  && digitCount >= PasswordRules.MinDigitCount;
        }
    }
}
