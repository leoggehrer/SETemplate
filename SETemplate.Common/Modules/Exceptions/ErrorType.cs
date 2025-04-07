//@CodeCopy
namespace SETemplate.Common.Modules.Exceptions
{
    /// <summary>
    /// ErrorType class represents the types of errors that can occur in the application.
    /// </summary>
    public static partial class ErrorType
    {
#if ACCOUNT_ON
        public const int InvalidEntitySet = 1;

        public const int InitAppAccess = 10;
        public const int AddAppAccess = 11;
        public const int InvalidAccount = 20;
        public const int InvalidIdentityName = 30;
        public const int InvalidPasswordSyntax = 40;
        
        public const int InvalidToken = 50;
        public const int InvalidSessionToken = 60;
        public const int InvalidJsonWebToken = 70;
        
        public const int InvalidEmail = 80;
        public const int InvalidPassword = 90;
        public const int NotLogedIn = 100;
        public const int NotAuthorized = 110;
        public const int AuthorizationTimeOut = 120;
        public const int MissingAuthorizeAttribute = 125;
        
#if ACCESSRULES_ON
        public const int InvalidAccessRuleEntityValue = 130;
        public const int InvalidAccessRuleAccessValue = 140;
        public const int InvalidAccessRuleAlreadyExits = 150;
        
        public const int AccessRuleViolationCanNotCreated = 160;
        public const int AccessRuleViolationCanNotRead = 170;
        public const int AccessRuleViolationCanNotChanged = 180;
        public const int AccessRuleViolationCanNotDeleted = 190;
#endif
#endif
        
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public const int InvalidId = 200;
        public const int InvalidPageSize = 210;
        
        public const int InvalidEntityInsert = 220;
        public const int InvalidEntityUpdate = 230;
        public const int InvalidEntityContent = 240;
        
        public const int InvalidControllerType = 250;
        public const int InvalidControllerObject = 260;
        
        public const int InvalidFacadeType = 270;
        public const int InvalidFacadeObject = 280;
        
        public const int InvalidOperation = 290;
        public const int EmailWasNotSent = 300;
        public const int InvalidConfirmation = 310;
#pragma warning restore CA2211 // Non-constant fields should not be visible
    }
}

