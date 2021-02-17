using System;
using System.IO;
using Cubicles.Utility.Helpers;

namespace Cubicles.Utility
{
    /// <summary>
    /// This class contains some input/output methods for workung with files and directories
    /// </summary>
    public static class IO
    {
        /// ValidatePath
        #region ValidatePath

        /// <summary>
        /// Validates specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>true if the path represents existing file or directory or parent directory</returns>
        public static bool ValidatePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                if (File.Exists(path))
                    return true;
            }
            catch (Exception)
            {
            }

            try
            {
                if (Directory.Exists(path))
                    return true;
            }
            catch (Exception)
            {
            }

            try
            {
                if (Directory.Exists(Path.GetDirectoryName(path)))
                    return true;
            }
            catch (Exception)
            {
            }

            return false;
        }

        #endregion

        /// CleanDirectory
        #region CleanDirectory

        /// <summary>
        /// Cleans specified directory
        /// </summary>
        /// <param name="directoryName">directory name</param>
        public static void CleanDirectory(string directoryName)
        {
            if (string.IsNullOrWhiteSpace(directoryName))
                return;

            try
            {
                if (!Directory.Exists(directoryName))
                    return;

                DirectoryInfo di = new DirectoryInfo(directoryName);
                LogHelper.LogDebug(directoryName);
                var files = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    try
                    {
                        File.Delete(file.FullName);
                    }
                    catch (Exception ex)
                    {
                        WPFNotifier.DebugError(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }

        #endregion

        /// CreateHiddenDirectory
        #region CreateHiddenDirectory

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directoryName"></param>
        public static void CreateHiddenDirectory(string directoryName, bool hide = true)
        {
            if (string.IsNullOrWhiteSpace(directoryName))
                return;

            try
            {
                DirectoryInfo di;

                if (!Directory.Exists(directoryName))
                    di = Directory.CreateDirectory(directoryName);
                else
                    di = new DirectoryInfo(directoryName);

                if (hide)
                    di.Attributes |= FileAttributes.Hidden | FileAttributes.Directory;
                else
                    di.Attributes ^= FileAttributes.Hidden;
            }
            catch (Exception ex)
            {
                LogHelper.Log(directoryName + " " + ex);
            }
        }

        #endregion

        /// IsFileLocked
        #region IsFileLocked

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string filename)
        {
            FileStream stream = null;
            
            try
            {
                FileInfo file = new FileInfo(filename);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            return false;
        }

        #endregion
    }
}
