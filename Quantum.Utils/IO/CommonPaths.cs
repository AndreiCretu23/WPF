using System;
using System.Runtime.InteropServices;

namespace Quantum.Utils
{
    public static class CommonPaths
    {
        public static string Root { get { return String.Empty; } }
        public static string Recent { get { return Environment.GetFolderPath(Environment.SpecialFolder.Recent); } }
        public static string Desktop { get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); } }
        public static string Documents { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); } }
        public static string Downloads { get { return GetDownloadsFolderPath(); } }
        public static string Music { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyMusic); } }
        public static string Pictures { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures); } }
        public static string Videos { get { return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos); } }

        public static string AppDataLocal { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); } }
        public static string AppDataRoaming { get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); } }

        #region Private

        public static string GetDownloadsFolderPath()
        {
            string DownloadsFullPath;
            SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), 0, IntPtr.Zero, out DownloadsFullPath);
            return DownloadsFullPath;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out string pszPath);

        #endregion Private
    }
}
