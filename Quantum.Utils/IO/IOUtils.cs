using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Quantum.Utils
{
    public static class IOUtils
    {

        #region Common

        public static FileType GetFileType(string path)
        {
            if (path == String.Empty) return FileType.Root;
            else if (IsDrive(path)) return FileType.Drive;
            else if (IsCommonDirectory(path)) return FileType.CommonDirectory;
            else if (Directory.Exists(path)) return FileType.Directory;
            else if (File.Exists(path)) return FileType.File;
            else return FileType.None;
        }



        public static IEnumerable<string> GetCommonDirectories()
        {
            yield return CommonPaths.Recent;
            yield return CommonPaths.Desktop;
            yield return CommonPaths.Documents;
            yield return CommonPaths.Downloads;
            yield return CommonPaths.Music;
            yield return CommonPaths.Pictures;
            yield return CommonPaths.Videos;
            yield return CommonPaths.AppDataLocal;
            yield return CommonPaths.AppDataRoaming;
        }
        
        public static string GetRootCommonDirectory(string path)
        {
            return GetCommonDirectories().SingleOrDefault(dir => dir.Contains(path));
        }
        
        public static bool IsCommonDirectory(string path)
        {
            return GetCommonDirectories().Contains(path);
        }

        public static bool IsSubDirOfSpecialDirectory(string path)
        {
            return Directory.Exists(path) && GetCommonDirectories().Any(dir => dir.Contains(path));
        }
        
        public static bool IsDrive(string path)
        {
            return Directory.GetLogicalDrives().Contains(path);
        }

       

        public static void Copy(string source, string destination, bool overwrite)
        {
            if (Directory.Exists(source))
            {
                if (Directory.Exists(destination))
                {
                    if (overwrite) Directory.Delete(destination);
                    else return;
                }

                DirectoryCopy(source, destination, true);
            }
            else if (File.Exists(source))
            {
                if (File.Exists(destination))
                {
                    if (overwrite) File.Delete(destination);
                    else return;
                }
                File.Copy(source, destination);
            }
            else
            {
                throw new Exception($"Source {source} does not exist!");
            }
        }

        public static void Move(string source, string destination, bool overwrite = false)
        {
            if (Directory.Exists(source))
            {
                Directory.Move(source, destination);
            }
            else if (File.Exists(source))
            {
                File.Copy(source, destination);
            }
            else
            {
                throw new Exception($"Source {source} does not exist!");
            }
        }

        public static void Delete(string source)
        {
            if (source == null)
            {
                throw new Exception("Null source path");
            }
            if (Directory.Exists(source))
            {
                ClearReadOnlyAttr(source);
                Directory.Delete(source, true);
            }
            else if (File.Exists(source))
            {
                ClearReadOnlyAttr(source);
                File.Delete(source);
            }
            else
            {
                throw new Exception("Source " + source + " does not exist");
            }
        }

        public static void DeleteIfExists(string source)
        {
            source.AssertParameterNotNull(nameof(source));
            if(File.Exists(source))
            {
                File.Delete(source);
            }
            else if(Directory.Exists(source))
            {
                Directory.Delete(source);
            }
        }


        
        public static bool DirectoryHasContent(string directoryPath)
        {
            var dirInfo = new DirectoryInfo(directoryPath);
            return dirInfo.Exists && (dirInfo.GetDirectories().Length > 0 || dirInfo.GetFiles().Length > 0);
        }

        public static void DeleteDirectoryContent(string path)
        {
            foreach (var dir in Directory.GetDirectories(path))
            {
                ClearReadOnlyAttr(dir);
                Directory.Delete(dir, true);
            }
            foreach (var file in Directory.GetFiles(path))
            {
                ClearReadOnlyAttr(file);
                File.Delete(file);
            }
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }



        private static void ClearReadOnlyAttr(string itemPath)
        {
            //attempt to remove the read only attribute from a file or folder
            try
            {
                if (Directory.Exists(itemPath) || File.Exists(itemPath))
                {

                    var attr = File.GetAttributes(itemPath);
                    if ((attr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        attr &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(itemPath, attr);
                    }
                }
            }
            catch (Exception) { }
        }


        #endregion Common


        #region Lock

        public static bool IsFileLocked(string filePath)
        {
            if (!File.Exists(filePath)) throw new IOException("Error : File does not exist");

            FileStream stream = null;

            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.Write, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            return false;
        }


        #endregion Lock


        #region GetFileSizeOnDisk
        /// <summary>
        /// Returns, in bytes, the actual file size of disk of the specified file.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "IDE0018")]
        public static long GetFileSizeOnDisk(string file)
        {
            if (!File.Exists(file)) return 0;

            var info = new FileInfo(file);
            uint dummy, sectorsPerCluster, bytesPerSector;
            int result = GetDiskFreeSpaceW(info.Directory.Root.FullName, out sectorsPerCluster, out bytesPerSector, out dummy, out dummy);
            if (result == 0) throw new Win32Exception();
            uint clusterSize = sectorsPerCluster * bytesPerSector;
            uint hosize;
            uint losize = GetCompressedFileSizeW(file, out hosize);
            long size;
            size = (long)hosize << 32 | losize;
            return ((size + clusterSize - 1) / clusterSize) * clusterSize;
        }

        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
           [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true, PreserveSig = true)]
        static extern int GetDiskFreeSpaceW([In, MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName,
           out uint lpSectorsPerCluster, out uint lpBytesPerSector, out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);

        #endregion GetFileSizeOnDisk


        #region ShowFilePropertiesPanel

        /// <summary>
        /// Opens the file properties panel. It is the equivalent of right clicking on a file/folder in windows explorer and selecting properties.
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        public static bool ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            return ShellExecuteEx(ref info);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;

        #endregion ShowFilePropertiesPanel


        #region Zip

        public static void UpZip(string zipFileName, string outputFolderName)
        {
            using (var zipFile = new ZipFile(zipFileName))
            {
                zipFile.AlternateEncoding = Encoding.UTF8;
                zipFile.AlternateEncodingUsage = ZipOption.AsNecessary;
                Directory.CreateDirectory(outputFolderName);
                zipFile.ExtractAll(outputFolderName);
            }
        }

        public static void ExtractFileFromZip(string zipFileName, string fileName, string outputDirectory, string password = null)
        {
            Directory.CreateDirectory(outputDirectory);
            using (var zipFile = ZipFile.Read(zipFileName))
            {
                foreach (ZipEntry entry in zipFile.Entries)
                {
                    if (entry.FileName == fileName)
                    {
                        if (password == null)
                        {
                            entry.Extract(outputDirectory, ExtractExistingFileAction.OverwriteSilently);
                        }
                        else
                        {
                            if (entry.UsesEncryption)
                            {
                                entry.ExtractWithPassword(outputDirectory, ExtractExistingFileAction.OverwriteSilently, password);
                            }
                        }
                        break;
                    }
                }
            }
        }

        public static void CreateZip(IEnumerable<string> sourceItems, string zipFileName, string password = null)
        {
            using (var zipFile = new ZipFile())
            {
                zipFile.AddDirectoryWillTraverseReparsePoints = true;
                zipFile.UseZip64WhenSaving = Zip64Option.Always;
                if (password != null) zipFile.Password = password;
                foreach (var item in sourceItems)
                {
                    if (File.Exists(item))
                    {
                        zipFile.AddFile(item, "");
                    }
                    else
                    {
                        zipFile.AddDirectory(item, new DirectoryInfo(item).Name);
                    }
                }
                zipFile.Save(zipFileName);
            }
        }

        #endregion Zip
    }

    public enum FileType
    {
        None,
        Root,
        Drive,
        CommonDirectory,
        Directory,
        File
    }
}
