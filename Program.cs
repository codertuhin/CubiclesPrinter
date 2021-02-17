using System;
using System.Threading;
using System.Windows.Forms;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinter.View;
using CubiclesPrinterLib;

namespace CubiclesPrinter
{
    /// <summary>
    /// 
    /// </summary>
    static class Program
    {
        /// Private Variables
        #region Private Variables

        /// <summary>
        /// This mutex prevents several instances of the application to be launched
        /// </summary>
        static Mutex mutex;

        #endregion

        /// Main
        #region Main

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        [STAThread]
        static void Main(string[] args)
        {
            //PrintJobData data = new PrintJobData() {PrintJobTitle = new PrintJobTitle("Canon MG3500 series Printer", "Test")};
            //PrintHelper.UpdateJobData(ref data);

            //var xxx = PrintHelper.GetPrintJobs("S77", "Canon MG3500 series Printer");
            //xxx = xxx;

            //string sss = "";
            //var xxx = PrintHelper.GetPrinterStatus("S77", "Canon MG3500 series Printer", out sss);
            //xxx = xxx;

            try
            {
                if (!SingleInstance())
                    return;

                LogHelper.Init(ConfigData.FilePath_AppUsageLog);
                LogHelper.Log("Started " + Application.ProductVersion);
                ConfigData.Init();
                Run(args);
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }

            try
            {
                LogHelper.Log("Exited");
            }
            catch (Exception)
            {
            }
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool SingleInstance()
        {
            //LogHelper.LogDebug();

            // use a global mutex to prevent application from launching multiple times
            mutex = new Mutex(false, ConfigData.MUTEX);

            if (!mutex.WaitOne(0, false))
            {
                Notifier.Warning("Application already run!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Runs the application
        /// </summary>
        /// <param name="args">command-line arguments</param>
        public static void Run(string[] args)
        {
            try
            {
                LogHelper.LogDebug();

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                Application.ThreadException += Application_ThreadException;
                
                /*
                Form f = new Form();
                f.Show();
                //string pn = "Brother MFC-L2740DW series";
                string pn = "Brother HL-3170CDW series";

                UISettingsExtractor extractor = new UISettingsExtractor(pn, DesktopHelper.GetDesktopWindow());
                extractor.Start();
                PrinterSettings ps = new PrinterSettings();
                ps.PrinterName = pn;
                PrintDialogHelper.EditPrinterSettings(ps, f);
                extractor.Stop();

                return;*/

                // check existence of command-line arguments
                if (args == null || args.Length < 1)
                {
                    LogHelper.LogDebug("Common run");
                    Application.Run(new MainForm());
                }
                else
                {
                    // check command-line arguments
                    foreach (string arg in args)
                    {
                        switch (arg.ToLower())
                        {
                            case "-sui":
                                ConfigData.Config_ShowUI = true;
                                break;
                            case "-sys":
                                ConfigData.Config_UseSystemPrinters = true;
                                break;
                            case "-nocheck":
                                ConfigData.Config_DontCheckUserBalance = true;
                                break;
                            case "-ic":
                                ConfigData.Config_InterceptPrintJobAndCancel = true;
                                break;
                        }
                    }

                    LogHelper.LogDebug("Run with args");
                    Application.Run(new MainForm());
                }
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }
        }

        #endregion

        /// Unhandled Exceptions
        #region Unhandled Exceptions

        /// <summary>
        /// 
        /// </summary>
        /// <param appName="sender"></param>
        /// <param appName="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Notifier.Error(e.ExceptionObject as Exception);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param appName="sender"></param>
        /// <param appName="e"></param>
        public static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                Notifier.Error(e.Exception);
            }
            catch
            {
            }
        }

        #endregion
    }
}
