namespace UHub.CoreLib.Security.Accounts
{
    //TODO: break into mutliple covariant enums
    //Allow consumers to properly handle codes by only seeing relevant outputs
    public enum AcctPswdResultCode : short
    {
        Success = 0,
        UnknownError = 1000,

        UserInvalid = 2000,
        LoginFailed = 2001,

        EmailInvalid = 4000,
        EmailEmpty = 4001,

        PswdInvalid = 5000,
        PswdEmpty = 5001,
        PswdNotChanged = 5002,
    }
}
