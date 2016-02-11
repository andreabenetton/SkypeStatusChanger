using SkypeStatusChanger.SkypeApi;

namespace SkypeStatusChanger.Online
{
    public interface IOnline
    {
        bool ChangeOnlineStatus(UserStatus toset);
    }
}