using System;
using System.IO;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Extensions;

namespace CubiclesPrinterLib
{
    /// <summary>
    /// ConfigData class.
    /// </summary>
    public class ConfigData
    {
        /// Variables
        #region Variables

        /// <summary>
        /// 
        /// </summary>
        public const string MUTEX = "2DCA3770-6100-4D47-A6B5-8E23B76685B2|M";

        public const int Interval_TimerRestart = 60000;
        public const int Interval_TimerGetPrinters = 60000;
        public const int Interval_TimerAllowedToPrint = 10000;
        public const int Interval_TimerAllowedToResumeJob = 10000;
        public const int Interval_TimerUpdatePrintJob = 1000;
        public const int Interval_TimerShowUI = 1000;
        public const int Interval_TimerPrinterManager = 60000;
        public const int Interval_TimerPrintJobWatcher = 1000;
        public const int Interval_TimerPrintEventWatcher = 1000;

        public const int Timeout_Request = 5000;

        public const string Pattern_File = "%c_%u_%Y%m%d_%H%n%s_%j.ps";
        public static string Path_Temp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Cubicles\Cubicles Printer\Temp");
        public static string Path_Monitor = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Cubicles\Cubicles Printer\Temp\Monitor");
        public static string Path_Processing = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Cubicles\Cubicles Printer\Temp\Processing");
        public static string Path_App = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"Cubicles\Cubicles Printer");
        public static string Path_User = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Cubicles\Cubicles Printer");
        public static string Path_AppExe = Path.Combine(Path_App, AppExeName);
        public static string Path_Logs = Path.Combine(Path_App, "Logs");

        public static string FilePath_PrinterConfig = @"C:\PrinterLogs\PrintersInfo.txt";
        public static string FilePath_PrintLog = @"C:\PrinterLogs\PrintLog.txt";

        public static string UserName = Environment.UserName;
        public static string MachineName = Environment.MachineName;

        public static string FilePath_DefaultPDFName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"MyFile.pdf");
        public static string FilePath_InnerConfig = Path.Combine(Path_App, @"CubiclesConfig.dat");
        public static string FilePath_RemoteConfig = Path.Combine(Path_App, @"CubiclesConfig.dat");
        public static string FilePath_EnvironmentConfig = Path.Combine(Path_App, @"EnvironmentConfig.dat");

        public static string FilePath_InstallLog = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows)), @"CubiclesPrinter_Installer.txt");
        public static string FilePath_UninstallLog = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.Windows)), @"CubiclesPrinter_Uninstaller.txt");
        public static string FilePath_AppUsageLog = Path.Combine(Path_Logs, @"AppUsageLog.txt");

        public const string PrinterName = "BLACK & WHITE PRINTER";
        public const string MonitorName = "CubiclesPrinterMonitor";
        public const string PortName = PrinterName + ":";
        public const string DriverName = "Cubicles Printer";

        public const string PrinterName2 = "COLOR PRINTER";
        public const string PortName2 = PrinterName2 + ":";
        public const string MonitorName2 = "CubiclesPrinterMonitor2";

        public const string AppName = "Cubicles Printer Control";
        public const string AppExeName = "CubiclesPrinterControl.exe";
        public const string AutorunName = "CubiclesPrinter";
        public static string AutorunPath = Path.Combine(Path_App, AppExeName);

        public static bool Config_ShowUI = false;
        public static bool Config_UseSystemPrinters = false;
        public static bool Config_DontCheckUserBalance = false;
        public static bool Config_InterceptPrintJobAndCancel = false;

        #endregion


        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        static ConfigData()
        {
            
        }

        /// <summary>
        /// Initializes data.
        /// </summary>
        public static void Init()
        {
            try
            {
                LoadConfig(FilePath_InnerConfig);
                LoadConfig(FilePath_RemoteConfig);

                IO.CreateHiddenDirectory(Path_App, false);
                IO.CreateHiddenDirectory(Path_User);
                IO.CreateHiddenDirectory(@"C:\PrinterLogs", false);
                IO.CreateHiddenDirectory(Path_Logs);
                IO.CreateHiddenDirectory(Path_Temp);
                IO.CreateHiddenDirectory(Path_Monitor);
                IO.CreateHiddenDirectory(Path_Processing);

                // clean monitor and processing directories at start
                IO.CleanDirectory(Path_Monitor);
                IO.CleanDirectory(Path_Processing);
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion

        /// LoadConfig
        #region LoadConfig

        /// <summary>
        /// Loads config data from the specified file.
        /// </summary>
        /// <param name="fileName">config file name</param>
        static void LoadConfig(string fileName)
        {
            LogHelper.LogDebug(fileName);

            if (string.IsNullOrWhiteSpace(fileName))
                return;

            try
            {
                if (!File.Exists(fileName))
                    return;

                LogHelper.LogDebug("Reading file");
                string text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                    return;

                string[] sstext = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (sstext == null || sstext.Length < 1)
                    return;

                LogHelper.LogDebug("Reading config data");
                foreach (string line in sstext)
                {
                    string[] liness = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (liness == null || liness.Length != 2)
                        continue;

                    string key = liness[0].Trim();
                    string value = liness[1].Trim();
                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    switch (key)
                    {
                        case "UserName":
                            UserName = value;
                            break;
                        case "MachineName":
                            MachineName = value;
                            break;
                        case "FilePath_PrintLog":
                            FilePath_PrintLog = value.FullPathFromRelative(Path_App);
                            LogHelper.LogDebug(FilePath_PrintLog);
                            break;
                        case "FilePath_PrinterConfig":
                            FilePath_PrinterConfig = value.FullPathFromRelative(Path_App);
                            break;
                        case "FilePath_RemoteConfig":
                            FilePath_RemoteConfig = value.FullPathFromRelative(Path_App);
                            break;
                    }
                }

                LogHelper.LogDebug("End");
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion
    }
}
