using SKYPE4COMLib;

namespace SkypeStatusChanger.SkypeApi
{
    public enum UserStatus
    {
        Unknown = TUserStatus.cusUnknown,
        Offline = TUserStatus.cusOffline,
        Online = TUserStatus.cusOnline,
        Away = TUserStatus.cusAway,
        NotAvailable = TUserStatus.cusNotAvailable,
        DoNotDisturb = TUserStatus.cusDoNotDisturb,
        Invisible = TUserStatus.cusInvisible,
        LoggedOut = TUserStatus.cusLoggedOut,
        SkypeMe = TUserStatus.cusSkypeMe,
    }
}
