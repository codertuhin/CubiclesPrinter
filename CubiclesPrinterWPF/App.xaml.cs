using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib;
using CubiclesPrinterWPF.Controller;
using System;
using System.Threading;
using System.Windows;

namespace CubiclesPrinterWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// Singleton
        #region Singleton

        /// Constants and Fields
        #region Constants and Fields

        /// <summary>The unique mutex name</summary>
        private const string UniqueMutexName = ConfigData.MUTEX;

        /// <summary>The mutex</summary>
        private Mutex mutex;

        /// <summary> is Owned </summary>
        bool isOwned;

        #endregion

        /// OnStartup
        #region OnStartup

        /// <summary>
        /// The event occurs on startup
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            AppOnStartup(this, e);
            base.OnStartup(e);
        }

        /// <summary>
        /// This event occurs with usual apps' closing
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (isOwned)
            {
                AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                System.Windows.Forms.Application.ThreadException -= Application_ThreadException;
                LogHelper.Log("Closed.");
                MainController.Singleton.Dispose();
            }
            base.OnExit(e);
        }

        #endregion

        /// AppOnStartup
        #region AppOnStartup

        /// <summary>The event occurs on startup.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args</param>
        private void AppOnStartup(object sender, StartupEventArgs e)
        {
            mutex = new Mutex(true, UniqueMutexName, out isOwned);

            GC.KeepAlive(mutex);

            // check if mutex is owned
            if (isOwned)
            { 
                // if mutex is owned then no other instances are active
                // create all the necessary data and run the app
                LogHelper.Init(ConfigData.FilePath_AppUsageLog);
                LogHelper.Log("Started " + System.Windows.Forms.Application.ProductVersion);
                ConfigData.Init();   

                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                System.Windows.Forms.Application.ThreadException += Application_ThreadException;

                ProcessArgs(e.Args);
                MainController.Singleton.Run();
                return;
            }

            // Application already run
            WPFNotifier.Warning("Application already run!");

            // Terminate this instance.
            Shutdown();
        }

        private void ProcessArgs(string[] args)
        {
            // check existence of command-line arguments
            if (args == null || args.Length < 1)
            {
                LogHelper.LogDebug("Common run");
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
            }

            //MainController.InitX();
        }

        #endregion

        #endregion

        // Unhandled Exceptions
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
                WPFNotifier.Error(e.ExceptionObject as Exception);
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
                WPFNotifier.Error(e.Exception);
            }
            catch
            {
            }
        }

        #endregion
    }
}
