namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctResultCode : short
    {
        Success = 0,
        UnknownError = 1000,

        UserInvalid = 2000,
        LoginFailed = 2001,

        UsernameInvalid = 3000,
        UsernameDuplicate = 3001,

        EmailInvalid = 4000,
        EmailEmpty = 4001,
        EmailDuplicate = 4002,
        EmailDomainInvalid = 4003,

        PswdInvalid = 5000,
        PswdEmpty = 5001,
        PswdNotChanged = 5002,

        NameEmpty = 6010,
        NameInvalid = 6011,
        MajorInvalid = 6020,
        MajorEmpty = 6021,
        YearInvalid = 6030,
        YearEmpty = 6030,
        CompanyInvalid = 6040,
        CompanyEmpty = 6041,
        JobTitleInvalid = 6050,
        PhoneInvalid = 6060,
        GradDateInvalid = 6070,

        RecoveryContextInvalid = 7000,
        RecoveryContextDisabled = 7001,
        RecoveryContextExpired = 7002,
        RecoveryContextDestroyed = 7003,
        RecoveryKeyInvalid = 7004,

        RefUIDEmpty = 8000,
        RefUIDInvalid = 8001,
        ConfirmTokenInvalid = 8002
    }
}
