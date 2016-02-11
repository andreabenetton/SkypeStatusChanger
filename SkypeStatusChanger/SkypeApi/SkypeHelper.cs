using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using SKYPE4COMLib;

namespace SkypeStatusChanger.SkypeApi
{
    public delegate void SkypeUserStatusChangedEventHandler(UserStatus status);

    public class SkypeHelper
    {
        private const string REGISTRY_SKYPE_PATH = @"Software\Skype\Phone";
        private const string SKYPE_PATH = "SkypePath";
        private const string SKYPE4_COM_DLL = @"Skype\Skype4COM.dll";
        public const string SKYPE_PROCESS_NAME = "Skype";
        public const int SKYPE_PROTOCOL = 7;
        private const bool WAIT_FOR_ATTACH = false;

        private readonly Skype _skype;

        public SkypeHelper()
        {
            // System.Runtime.InteropServices.COMException
            _skype = new Skype();
            ((_ISkypeEvents_Event)_skype).AttachmentStatus += SkypeAttachmentStatus;
            _skype.UserStatus += SkypeUserStatus;

            _skype.Attach(SKYPE_PROTOCOL, WAIT_FOR_ATTACH);        
        }

        public bool IsAttached { get; private set; }

        public bool IsLoggedIn()
        {
            bool logged = false;
            try
            {
                logged = _skype.CurrentUserStatus != TUserStatus.cusLoggedOut;
            }
            catch (Exception)
            {

            }
            return logged;

            
        }

        public static bool IsSkypeRunning()
        {
            return Process.GetProcessesByName(SKYPE_PROCESS_NAME).Length > 0;
        }

        public static bool IsSkypeInstalled()
        {
            using (RegistryKey skypePathKey = Registry.CurrentUser.OpenSubKey(REGISTRY_SKYPE_PATH))
            {
                return skypePathKey != null && skypePathKey.GetValue(SKYPE_PATH) != null;
            }
        }

        public static string GetSkype4ComPath()
        {
            string commonProgramFilesPath = Environment.Is64BitOperatingSystem
                                          ? Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86)
                                          : Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);

            return Path.Combine(commonProgramFilesPath, SKYPE4_COM_DLL);
        }

        public event SkypeUserStatusChangedEventHandler SkypeUserStatusChanged;

        protected virtual void OnSkypeUserStatusChanged(UserStatus status)
        {
            var handler = SkypeUserStatusChanged;
            handler?.Invoke(status);
        }

        public bool SetUserStatus(UserStatus status)
        {
            try
            {
                _skype.ChangeUserStatus((TUserStatus)status);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public string GetAccountName()
        {
            string toret = "";
            try
            {
                toret = _skype.CurrentUser.Handle;
            }
            catch (Exception)
            {
                // ignored
            }
            return toret;
        }

        public UserStatus GetUserStatus()
        {
            return (UserStatus)_skype.CurrentUserStatus;
        }

        private void SkypeUserStatus(TUserStatus status)
        {
            OnSkypeUserStatusChanged((UserStatus)status);
        }

        private void SkypeAttachmentStatus(TAttachmentStatus status)
        {
            switch (status)
            {
                case TAttachmentStatus.apiAttachAvailable:
                    _skype.Attach(SKYPE_PROTOCOL, WAIT_FOR_ATTACH);
                    break;

                case TAttachmentStatus.apiAttachSuccess:
                    IsAttached = true;
                    break;
            }
        }
    }
}
