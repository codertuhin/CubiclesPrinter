using System;
using System.Diagnostics;

namespace CubiclesPrinterSetupCustomAction.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public static void KillProcess(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
                return;

            LogHelper.Log("KillProcess Start.");

            try
            {
                var pp = Process.GetProcessesByName(processName);
                foreach (var p in pp)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Log(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            LogHelper.Log("KillProcess End.");
        }
    }
}
