//@BaseCode
namespace SETemplate.Common.Modules.Security
{
    /// <summary>
    /// Provides rules and settings for password validation.
    /// </summary>
    public static partial class PasswordRules
    {
        #region Class members
        /// <summary>
        /// Initializes the <see cref="PasswordRules"/> class.
        /// </summary>
        static PasswordRules()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This is a partial method that is called before the constructor of the class is called.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a partial method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class members
        
        #region Rule definitions
        /// <summary>
        /// Gets the minimum length value.
        /// </summary>
        public static int MinimumLength => 6;
        /// <summary>
        /// Gets the maximum length of the property.
        /// </summary>
        public static int MaximumLength => 30;
        ///<summary>
        /// Gets the minimum letter count.
        ///</summary>
        public static int MinLetterCount => MinLowerLetterCount + MinUpperLetterCount;
        /// <summary>
        /// Gets the minimum count of uppercase letters allowed.
        /// </summary>
        public static int MinUpperLetterCount => 1;
        /// <summary>
        /// Represents the minimum count of lowercase letters that a string must contain.
        /// </summary>
        public static int MinLowerLetterCount => 1;
        /// <summary>
        /// Gets the minimum count of special letters allowed.
        /// </summary>
        public static int MinSpecialCharCount => 0;
        /// <summary>
        /// Gets the minimum number of digits allowed.
        /// </summary>
        public static int MinDigitCount => 0;
        
        public const int MinLoginFails = 0;
        public const int MaxLoginFails = 20;
        
        private static int allowedLoginFails = 10;
        
        /// <summary>
        /// Gets or sets the maximum number of allowed login fails.
        /// </summary>
        public static int AllowedLoginFails
        {
            get => allowedLoginFails;
            set
            {
                if (value >= MinLoginFails && value <= MaxLoginFails)
                {
                    allowedLoginFails = value;
                }
            }
        }
        
        #endregion Rule definitions
        /// <summary>
        /// Gets the syntax roles for the property.
        /// </summary>
        public static string SyntaxRoles => $"{nameof(MinimumLength)}: {MinimumLength} "
          + $"{nameof(MaximumLength)}: {MaximumLength} "
          + Environment.NewLine
          + $"{nameof(MinLetterCount)}: {MinLetterCount} "
          + $"{nameof(MinDigitCount)}: {MinDigitCount} "
          + Environment.NewLine
          + $"{nameof(MinUpperLetterCount)}: {MinUpperLetterCount} "
          + $"{nameof(MinLowerLetterCount)}: {MinLowerLetterCount} "
          + Environment.NewLine
          + $"{nameof(MinSpecialCharCount)}: {MinSpecialCharCount} ";
    }
}
