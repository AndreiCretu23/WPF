using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Quantum.Utils
{
    public static class AppInfo
    {
        public static string ApplicationName { get { return Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName); } }

        public static string ApplicationNameWithExtension { get { return AppDomain.CurrentDomain.FriendlyName; } }

        public static string ApplicationDirectory { get { return AppDomain.CurrentDomain.BaseDirectory; } }

        public static string ApplicationPath { get { return Path.Combine(ApplicationDirectory, ApplicationNameWithExtension); } }

        public static string ApplicationRepository { get { return Path.Combine(CommonPaths.AppDataRoaming, ApplicationName); } }

        public static string ApplicationConfigRepository { get { return Path.Combine(ApplicationRepository, "Config"); } }

        public static int ApplicationInstanceCount { get { return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count(); } }

        static AppInfo()
        {
            if(!Directory.Exists(ApplicationRepository)) {
                Directory.CreateDirectory(ApplicationRepository);
            }
            if(!Directory.Exists(ApplicationConfigRepository)) {
                Directory.CreateDirectory(ApplicationConfigRepository);
            }
        }

    }
}
