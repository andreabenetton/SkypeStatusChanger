using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Win32;
using SkypeStatusChanger.Common;
using SkypeStatusChanger.Online;
using SkypeStatusChanger.SkypeApi;

namespace SkypeStatusChanger.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private const string SKYPE_NOT_STARTED = "Skype not started";
        private const string SKYPE_PROCESS_NAME_EXT = SkypeHelper.SKYPE_PROCESS_NAME + ".exe";

        private List<string> _skypeStatuses;
        private string _lockSkypeStatus;
        private string _unlockSkypeStatus;
        private string _currentSkypeStatus = SKYPE_NOT_STARTED;

        private bool _usePreviousStatus = true;
        private bool _startOnSkypeStartup;
        private bool _isSkypeRunning;

        private SkypeHelper _skypeHelper;
        private IOnline _onlineStatus;
        private UserStatus _prevUserStatus;

        public MainWindowViewModel()
        {
            if (SkypeHelper.IsSkypeRunning())
            {
                InitSkypeHelper();
                IsSkypeRunning = true;
            }

            SkypeStatuses = new List<string>
                {
                    UserStatus.Online.ToString(),  
                    UserStatus.Away.ToString(),
                    UserStatus.DoNotDisturb.ToString(),
                    UserStatus.Invisible.ToString(),
                    UserStatus.Offline.ToString()
                };

            LockSkypeStatus = UserStatus.Away.ToString();
            UnlockSkypeStatus = UserStatus.Online.ToString();

            InitSystemEvents();
        }

        #region UI Properties

        public List<string> SkypeStatuses
        {
            get { return _skypeStatuses; }
            set { SetValue(ref _skypeStatuses, value, () => SkypeStatuses); }
        }

        public string LockSkypeStatus
        {
            get { return _lockSkypeStatus; }
            set { SetValue(ref _lockSkypeStatus, value, () => LockSkypeStatus); }
        }

        public string UnlockSkypeStatus
        {
            get { return _unlockSkypeStatus; }
            set { SetValue(ref _unlockSkypeStatus, value, () => UnlockSkypeStatus); }
        }

        public string CurrentSkypeStatus
        {
            get { return _currentSkypeStatus; }
            set { SetValue(ref _currentSkypeStatus, value, () => CurrentSkypeStatus); }
        }

        public bool UsePreviousStatus
        {
            get { return _usePreviousStatus; }
            set { SetValue(ref _usePreviousStatus, value, () => UsePreviousStatus); }
        }

        public bool StartOnSkypeStartup
        {
            get { return _startOnSkypeStartup; }
            set { SetValue(ref _startOnSkypeStartup, value, () => StartOnSkypeStartup); }
        }

        public bool IsSkypeRunning
        {
            get { return _isSkypeRunning; }
            set { SetValue(ref _isSkypeRunning, value, () => IsSkypeRunning); }
        }

        #endregion

        public SystemEventsWatcher OsEventsWatcher { get; private set; }

        private void InitSkypeHelper()
        {
            _skypeHelper = new SkypeHelper();
            _skypeHelper.SkypeUserStatusChanged += SkypeUserStatusChanged;
            CurrentSkypeStatus = _skypeHelper.IsAttached ? _skypeHelper.GetUserStatus().ToString() : UserStatus.LoggedOut.ToString();
        }

        private void InitSystemEvents()
        {
            SystemEvents.SessionSwitch += SystemEventsSessionSwitch;
            OsEventsWatcher = new SystemEventsWatcher();
            OsEventsWatcher.AddProcessStartEventHandler(SKYPE_PROCESS_NAME_EXT, SkypeStarted);
            OsEventsWatcher.AddProcessStopEventHandler(SKYPE_PROCESS_NAME_EXT, SkypeStopped);
        }

        private void SkypeUserStatusChanged(UserStatus status)
        {
            CurrentSkypeStatus = status.ToString();
            if (_onlineStatus==null && _skypeHelper.IsLoggedIn())
            {
                _onlineStatus = new FtpOnline(_skypeHelper.GetAccountName());
            }
            _onlineStatus?.ChangeOnlineStatus(status);
        }

        private static UserStatus ParseUserStatus(string status)
        {
            return (UserStatus)Enum.Parse(typeof(UserStatus), status);
        }

        #region System events handlers

        private void SystemEventsSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (!IsSkypeRunning) return;

            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    if (UsePreviousStatus)
                        _prevUserStatus = ParseUserStatus(CurrentSkypeStatus);

                    _skypeHelper.SetUserStatus(ParseUserStatus(LockSkypeStatus));
                    break;

                default:
                    _skypeHelper.SetUserStatus(UsePreviousStatus ? _prevUserStatus : ParseUserStatus(UnlockSkypeStatus));
                    break;
            }
        }

        private void SkypeStopped()
        {
            _skypeHelper = null;
            CurrentSkypeStatus = SKYPE_NOT_STARTED;
            _onlineStatus?.ChangeOnlineStatus(UserStatus.LoggedOut);
            _onlineStatus = null;
            IsSkypeRunning = false;
        }

        private void SkypeStarted()
        {
            InitSkypeHelper();
            IsSkypeRunning = true;
        }

        #endregion
    }
}