using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Cubicles.Utility.Helpers
{
    /// <summary>
    /// This class allows to log data
    /// </summary>
    public static class LogHelper
    {
        /// Variables
        #region Variables

        /// <summary> Log file name </summary>
        public static string LogFileName = "log.txt";

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Initializes log helper
        /// </summary>
        /// <param name="fileName">log file name</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Init(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            try
            {
                File.Delete(fileName);
            }
            catch (Exception)
            {
            }

            LogFileName = fileName;
        }

        #endregion

        /// Log
        #region Log

        /// <summary>
        /// Logs message
        /// </summary>
        /// <param name="msg">message</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Log(string msg)
        {
            try
            {
                using (FileStream fs = new FileStream(LogFileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Unicode))
                    {
                        sw.WriteLine(string.Format("{0} - {1}{2}", DateTime.Now, msg, Environment.NewLine));
                        sw.Flush();
                    }
                }
            }
            catch
            {}
        }

        /// <summary>
        /// Logs message
        /// </summary>
        /// <param name="msg">message</param>
        /// <param name="index"></param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogDebug(string msg, int index = 2)
        {
#if (DEBUG)
                Log(string.Format("{1}{0}{2}", Environment.NewLine, ReflectionHelper.GetCallingString(index), msg));
#endif
        }

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="ex">exception</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogDebug(Exception ex = null)
        {
#if (DEBUG)
            if (ex == null)
                LogDebug("", 3);
            else
                LogDebug(ex.ToString(), 3);
#endif
        }

        /// <summary>
        /// Logs exception
        /// </summary>
        /// <param name="ex">exception</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Log(Exception ex)
        {
            if (ex == null)
                return;

            Log(ex.ToString());
        }
        
        #endregion
    }
}
