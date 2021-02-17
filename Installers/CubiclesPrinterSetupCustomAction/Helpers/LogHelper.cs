using System;
using System.Configuration;
using Cubicles.Extensions;
using CubiclesPrinterLib;

namespace CubiclesPrinterSetupCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    public static class LogHelper
    {
        /// Variables
        #region Variables

        /// <summary>
        /// 
        /// </summary>
        public static string LogFileName = ConfigData.FilePath_InstallLog;

        #endregion

        /// Log
        #region Log

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="filename"></param>
        public static void Log(string msg, string filename = null)
        {
            if (string.IsNullOrWhiteSpace(filename))
                filename = LogFileName;

            using (var sw = new System.IO.StreamWriter(filename, true))
            {
                sw.Write(string.Format("{0} - {1}{2}", DateTime.Now, msg, Environment.NewLine));
                sw.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="filename"></param>
        public static void LogDebug(string msg, string filename = null)
        {
            if (ConfigurationManager.AppSettings["DebugMode"].ToBool())
            {
                Log(msg, filename);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void LogDebug(Exception ex)
        {
            if (ex == null)
                return;

            LogDebug(ex.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public static void Log(Exception ex)
        {
            if (ex == null)
                return;

            Log(ex.Message);
        }
        
        #endregion
    }
}
