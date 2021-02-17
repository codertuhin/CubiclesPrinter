using System;
using System.IO;

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
        public const string DriverName2 = "Cubicles Printer";

        public const string AppName = "Cubicles Printer Control";
        public const string AppExeName = "CubiclesPrinterControl.exe";
        public const string AutorunName = "CubiclesPrinter";
        public static string AutorunPath = Path.Combine(Path_App, AppExeName);

        public static bool Config_ShowUI = false;
        public static bool Config_UseSystemPrinters = false;
        public static bool Config_DontCheckUserBalance = false;
        public static bool Config_InterceptPrintJobAndCancel = false;

        #endregion
    }
}
