using System.Diagnostics;

namespace SkypeStatusChanger.Common
{
    public static class ComServerHelper
    {
        private const string REG_SVR32 = "regsvr32";

        public static bool RegisterComServer(string serverPath)
        {
            return StartRegSvr32Process("/s ", serverPath);
        }

        public static bool UnregisterComServer(string serverPath)
        {
            return StartRegSvr32Process("/u /s", serverPath);
        }

        private static bool StartRegSvr32Process(string parameters, string serverPath)
        {
            using (var proc = new Process())
            {
                proc.StartInfo = new ProcessStartInfo(REG_SVR32, string.Concat(parameters, GetEnclosedByQuotes(serverPath)));
                return proc.Start();
            }
        }

        private static string GetEnclosedByQuotes(string s)
        {
            return string.Concat('"', s, '"');
        }
    }
}
