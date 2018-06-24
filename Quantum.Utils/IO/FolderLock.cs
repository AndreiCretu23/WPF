using System;
using System.IO;

namespace Quantum.Utils
{
    public class FolderLock : IDisposable
    {
        public string FolderName { get; private set; }
        public string LockFileName { get; private set; }
        private FileStream LockStream { get; set; }

        private const string Lock = ".lock";

        public FolderLock(string folderName)
        {
            if (!Directory.Exists(folderName))
            {
                throw new IOException("Directory does not exist");
            }

            FolderName = folderName;
            LockFileName = $"{folderName}/{Lock}";

            if (File.Exists(LockFileName))
            {
                throw new Exception("The directory is already locked!");
            }

            LockStream = File.Open(LockFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        }

        public void Dispose()
        {
            if (LockStream != null)
            {
                lock (this)
                {
                    if (LockStream != null)
                    {
                        LockStream.Close();
                        LockStream = null;
                    }
                }
            }

            try
            {
                var fileInfo = new FileInfo(LockFileName);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }
            }
            catch (Exception) { }
        }
    }
}
