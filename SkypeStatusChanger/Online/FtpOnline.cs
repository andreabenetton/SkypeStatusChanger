using System;
using System.Net;
using System.Net.FtpClient;
using System.Threading;
using SkypeStatusChanger.SkypeApi;

namespace SkypeStatusChanger.Online
{
    public class FtpOnline : IOnline
    {
        private readonly string _account;



        public FtpOnline(string account)
        {
            _account = account;
        }

        private string _ftpSkypeDir = Properties.Settings.Default.FTPSkypeDir;
        private string _ftpHost = Properties.Settings.Default.FTPHost;
        private string _ftpUsername = Properties.Settings.Default.FTPUsername;
        private string _ftpPassword = Properties.Settings.Default.FTPPassword;

        private FtpClient GetOpenFTP()
        {
            FtpClient cl = null;
            try
            {
                cl = new FtpClient
                {
                    Credentials = new NetworkCredential(_ftpUsername, _ftpPassword),
                    Host = _ftpHost
                };
                cl.Connect();
            }
            catch (Exception)
            {
                return null;
            }
            return cl;
        }

        private bool UnsetOnlineStatus(FtpClient cl)
        {
            UserStatus onlineStatus = UserStatus.Unknown;
            try
            {
                string[] sa;
                sa = cl.GetNameListing(_ftpSkypeDir + _account);

                foreach (var iterateStatus in Enum.GetValues(typeof (UserStatus)))
                {
                    bool found = false;

                    foreach (var s in sa)
                    {
                        int indir;
                        if (int.TryParse(s.Substring(0,s.IndexOf(".", StringComparison.Ordinal)), out indir))
                        {
                            if (indir == (int) iterateStatus)
                            {
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        onlineStatus = (UserStatus) iterateStatus;
                        break;
                    }
                }

                if (cl.FileExists(_ftpSkypeDir + _account + "/current.png"))
                {
                    cl.Rename(_ftpSkypeDir + _account + "/current.png",
                        _ftpSkypeDir + _account + "/" + (int)onlineStatus + ".png");
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool ChangeFTPStatus(UserStatus toset)
        {
            try
            {
                using (FtpClient conn = GetOpenFTP())
                {
                    UnsetOnlineStatus(conn);
                    conn.Rename(_ftpSkypeDir + _account + "/" + (int)toset + ".png", _ftpSkypeDir + _account + "/current.png");
                    conn.Disconnect();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool ChangeOnlineStatus(UserStatus toset)
        {
            Thread thread = new Thread(() => ChangeFTPStatus(toset));
            thread.Start();
            return true;
        }
    }
}