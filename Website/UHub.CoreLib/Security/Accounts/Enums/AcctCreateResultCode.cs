namespace UHub.CoreLib.Security.Accounts
{
    public enum AcctCreateResultCode : short
    {
        Success = 0,
        UnknownError = 1000,

        UserInvalid = 2000,

        UsernameInvalid = 3000,
        UsernameDuplicate = 3001,

        EmailInvalid = 4000,
        EmailEmpty = 4001,
        EmailDuplicate = 4002,
        EmailDomainInvalid = 4003,

        PswdInvalid = 5000,
        PswdEmpty = 5001,
        PswdNotChanged = 5002,

        NameInvalid = 6000,
        MajorInvalid = 6001,
        YearInvalid = 6002,
        CompanyInvalid = 6003,
    }
}
