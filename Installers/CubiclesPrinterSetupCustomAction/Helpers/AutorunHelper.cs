using System;
using CubiclesPrinterLib;
using Microsoft.Win32;

namespace CubiclesPrinterSetupCustomAction
{
    /// <summary>
    /// AutorunHelper class.
    /// </summary>
    public static class AutorunHelper
    {
        /// Methods
        #region Methods

        /// <summary>
        /// Adds application to startup registry key.
        /// </summary>
        public static void AddToStartup()
        {
            try
            {
                LogHelper.Log("Adding Cubicles Printer to Startup");
                var regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (regKey != null)
                    regKey.SetValue(ConfigData.AutorunName, (string)ConfigData.AutorunPath, RegistryValueKind.String);
            }
            catch
            {
            }
            
        }

        /// <summary>
        /// Removes application from startup registry key.
        /// </summary>
        public static void RemoveFromStartup()
        {
            try
            {
                LogHelper.Log("Removing Cubicles Printer from Startup");
                var regKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (regKey != null)
                    regKey.DeleteValue(ConfigData.AutorunName);
            }
            catch
            {
            }
        }

        #endregion
    }
}
