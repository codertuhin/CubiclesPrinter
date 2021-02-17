using Cubicles.API;
using Cubicles.API.JsonClasses;
using Cubicles.Events;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;
using CubiclesPrinterLib.Utility;
using CubiclesPrinterWPF.View;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Threading;
using CubiclesPrinterWPF.ViewModel;

namespace CubiclesPrinterWPF.Controller
{
    /// <summary>
    /// This class serves as the controller of the application's main logic
    /// </summary>
    internal sealed class MainController : IDisposable
    {
        /// Private Variables
        #region Private Variables

        /// <summary> File Monitor </summary>
        IOMonitorHelper fileMonitor = new IOMonitorHelper();

        /// <summary> Watches for print events </summary>
        private PrintEventWatcher localPrintEventWatcher;

        /// <summary> Singleton </summary>
        private static MainController _singleton;

        /// <summary> List of available printers </summary>
        private Printers _availablePrinters = null;

        /// <summary> Print job watchers </summary>
        private PrintJobWatchers _watchers = new PrintJobWatchers();

        /// <summary> Retry timer </summary>
        private Timer _timerRetry;

        /// <summary> GetPrinters timer </summary>
        private Timer _timerGetPrinters;

        /// <summary> Current dispatcher </summary>
        private Dispatcher _dispatcher;

        /// <summary> Special flag for showing only one message about cancelling the jobs </summary>
        private bool cancelledShowing = false;

        /// <summary> List of Printing Control Windows </summary>
        private List<PrintingControlWindow> listOfWindows = new List<PrintingControlWindow>(); 

        #endregion

        /// Properties
        #region Properties

        /// <summary> Singleton </summary>
        public static MainController Singleton { get { return _singleton; } }

        /// <summary> Has Available Printers </summary>
        public bool HasAvailablePrinters { get { return _availablePrinters == null ? false : _availablePrinters.Count < 1 ? false : true;  }}

        /// <summary> Current dispatcher </summary>
        public Dispatcher Dispatcher { get { return _dispatcher;}}

        /// <summary> List of available printers </summary>
        public Printers AvailablePrinters
        {
            get
            {
                try
                {
                    LogHelper.LogDebug("Seeking printers");

                    // check the current list of available printers
                    if (_availablePrinters == null || _availablePrinters.Count < 1)
                    {
                        // check if use all the system printers (flag for local testing) 
                        if (ConfigData.Config_UseSystemPrinters)
                            _availablePrinters = PrintHelper.GetPrinters();
                        else
                            // get printers via API
                            _availablePrinters = APIWrapper.GetPrinters(Environment.MachineName);
                    }

                    // check if the list of available printers still empty
                    if (_availablePrinters == null || _availablePrinters.Count < 1)
                    {
                        if (ConfigData.Config_UseSystemPrinters)
                            LogHelper.LogDebug("No system printers found");
                        else
                            LogHelper.LogDebug("No printers found : Server ain't responding");

                        // calculate the path to the config file
                        string path = ConfigData.FilePath_PrinterConfig;
                        LogHelper.LogDebug("ExePath : " + System.Windows.Forms.Application.ExecutablePath);
                        if (string.IsNullOrWhiteSpace(Path.GetPathRoot(ConfigData.FilePath_PrinterConfig)))
                            path = Path.Combine(Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath), ConfigData.FilePath_PrinterConfig);

                        // get printers from the local text config file
                        _availablePrinters = PrintHelper.GetPrintersFromConfig(path);
                    }
                    else
                    {
                        _availablePrinters.Save(ConfigData.FilePath_PrinterConfig);
                    }

                    LogHelper.LogDebug("Seeking printers : " + (_availablePrinters == null ? "no printers" : _availablePrinters.Count.ToString()));
                }
                catch (Exception ex)
                {
                    WPFNotifier.Error(ex);
                }

                // check if the list of available printers still empty; even now
                if (_availablePrinters == null || _availablePrinters.Count < 1)
                {
                    // write log that printers not found 
                    LogHelper.LogDebug("No printers found");

                    // initiate printer seeking procedure via timer
                    _timerGetPrinters.Start();
                }

                return _availablePrinters;
            }
        }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        private MainController()
        {
            InitTimer();

            Init();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        static MainController()
        {
            // create singleton instance
            _singleton = new MainController();
        }

        /// <summary>
        /// Initializes timers
        /// </summary>
        private void InitTimer()
        {
            try
            {
                // init retry timer
                _timerRetry = new Timer();
                _timerRetry.Enabled = false;
                _timerRetry.AutoReset = false;
                _timerRetry.Interval = ConfigData.Interval_TimerRestart;
                _timerRetry.Elapsed += timerRetry_Elapsed;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            try
            {
                // init get printers timer
                _timerGetPrinters = new Timer();
                _timerGetPrinters.Enabled = false;
                _timerGetPrinters.AutoReset = false;
                _timerGetPrinters.Interval = ConfigData.Interval_TimerGetPrinters;
                _timerGetPrinters.Elapsed += timerGetPrinters_Elapsed;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        /// <summary>
        /// Removes related jobs
        /// </summary>
        public void RemoveJobs()
        {
            // get available printers
            var xxx = AvailablePrinters;

            // remove print jobs of the current host for each available printer
            RemoveJobs1(Environment.MachineName);
        }

        /// <summary>
        /// Removes related jobs from the specified host
        /// </summary>
        /// <param name="host">host</param>
        private void RemoveJobs1(string host)
        {
            // check the host
            if (string.IsNullOrWhiteSpace(host))
                return;

            // check available printers
            if (_availablePrinters == null || _availablePrinters.Count < 1)
                return;

            // iterate thrtough all the available printers and cancel print jobs
            foreach (Printer printer in _availablePrinters)
                PrintHelper.CancelPrintJobsByUser(host, Environment.MachineName, printer.Name, Environment.UserName);
        }

        /// <summary>
        /// Runs the controller tracking logic
        /// </summary>
        public void Run()
        {
            // starts tracking
            Start();

            // test part which shows UI for some local test file
            if (ConfigData.Config_ShowUI)
            {
                localPrintEventWatcher.AllowedPrintersTitles.Add(new PrintJobTitle(){Document = "Test"});
                LaunchPrintControl("Test.ps", false);
            }
        }

        /// <summary>
        /// Initializes data
        /// </summary>
        private void Init()
        {
            try
            {
                LogHelper.LogDebug();

                // get current dispatcher for work with UI
                _dispatcher = Dispatcher.CurrentDispatcher;                

                // load config files
                API.LoadConfig(ConfigData.FilePath_InnerConfig); // local config or 1st config
                API.LoadConfig(ConfigData.FilePath_RemoteConfig); // remote config or 2nd config

                // removes old print jobs of the available printers
                RemoveJobs();

                // establish call-backs
                PrintHelper.SkipMethod = SkipDocument;
                PrintHelper.AddWatcher = AddWatcher;

                // subscribe to the file monitor event
                fileMonitor.FileFound += fileMonitor_FileFound;

                // create list of (virtual) printers to be excluded from the track process
                List<string> allp = new List<string> { ConfigData.PrinterName, ConfigData.PrinterName2 };

                // set up local watcher
                localPrintEventWatcher = new PrintEventWatcher(Environment.MachineName, Environment.MachineName, false, allp, "0.001");
                localPrintEventWatcher.PrintJobPaused += LocalPrintEventWatcherPrintJobPaused;   
                localPrintEventWatcher.PrintJobStarted += LocalPrintEventWatcherPrintJobStarted;
                localPrintEventWatcher.PrintJobCompleted += LocalPrintEventWatcherPrintJobCompleted;
                localPrintEventWatcher.PrintJobCancelled += LocalPrintEventWatcherPrintJobCancelled;

                // set up allowed printers to skip
                localPrintEventWatcher.AllowedPrintersToSkip = new List<string>();
                localPrintEventWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName);
                localPrintEventWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName2);

                // initialize watcher
                localPrintEventWatcher.Init();

                // update available printers
                UpdateAvailablePrinters();

                // init print job watchers
                _watchers = new PrintJobWatchers();
                _watchers.IssueFound += _watchers_IssueFound;
                _watchers.JobCompleted += _watchers_JobCompleted;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
                _timerRetry.Start();
            }

            // set default printer
            SetDefaultPrinter();
        }
        
        #endregion

        /// Watcher Subscriptions
        #region Watcher Subscriptions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalPrintEventWatcherPrintJobPaused(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug();

            // check the event args
            if (e != null && e.Data != null)
            {
                // launch UI for the specified data
                LaunchPrintControl(e.Data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalPrintEventWatcherPrintJobCompleted(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug();

            // check the event args
            if (e != null && e.Data != null)
            {
                // check the watchers and remove the watcher for the specified data
                if (_watchers != null && _watchers.Count > 0)
                    _watchers.RemoveWatcher(e.Data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalPrintEventWatcherPrintJobCancelled(object sender, PrintJobDataEventArgs e)
        {
            // check the event args
            if (e != null && e.Data != null)
            {
                LogHelper.LogDebug("Cancelled WPF! " + e.Data.PrintJobTitle.MegaToString());

                // check the watchers and remove the watcher for the specified data
                if (_watchers != null && _watchers.Count > 0)
                    _watchers.RemoveWatcher(e.Data);

                // check if able to show job cancelled message 
                if (!cancelledShowing)
                {
                    // show only one message about cancelling the job
                    cancelledShowing = true;
                    WPFNotifier.Warning(string.Format("You are not allowed to choose this printer.{0}You have to use {1} or {2} instead.{0}Printing cancelled.", Environment.NewLine, ConfigData.PrinterName, ConfigData.PrinterName2));
                    cancelledShowing = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LocalPrintEventWatcherPrintJobStarted(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug();

            // check the event args
            if (e != null && e.Data != null && e.Data.PrintJobTitle != null)
                // add watcher for the specified data
                _watchers.AddWatcher(new PrintJobWatcher(e.Data.PrintJobTitle));
        }

        #endregion

        /// FileMonitor Subscriptions
        #region FileMonitor Subscriptions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void fileMonitor_FileFound(object sender, DataEventArgs e)
        {
            LogHelper.LogDebug();

            // check the event args
            if (e != null && e.Data != null)
            {
                // launch UI for the virtual printer when the file arrived
                LaunchPrintControl(e.Data as string);
            }
        }

        #endregion

        /// Timers
        #region Timers

        /// <summary>
        /// Reinitializes data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerRetry_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.LogDebug();

            // stop the timer
            _timerRetry.Stop();

            // reinitialize the data
            Init();
        }

        /// <summary>
        /// Updates available printers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerGetPrinters_Elapsed(object sender, ElapsedEventArgs e)
        {
            LogHelper.LogDebug();

            // stop the timer
            _timerGetPrinters.Stop();

            // check available printers
            if (_availablePrinters == null || _availablePrinters.Count < 1)
                // update available printers
                UpdateAvailablePrinters();
        }

        #endregion

        /// Watcher SSS Event Handlers
        #region Watcher SSS Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _watchers_JobCompleted(object sender, PrintJobDataEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _watchers_IssueFound(object sender, PrinterIssueEventArgs e)
        {
            LogHelper.LogDebug();
            try
            {
                // check the event args
                if (e != null)
                {
                    LogHelper.LogDebug(e.Issue.Issue());
                    if (e.Issue != PrinterTrouble.None)
                    {
                        // notify about issue via API
                        APIWrapper.PrinterIssue(Environment.MachineName, Environment.UserName, e.PrinterName, e.Issue);

                        // show error message to user
                        WPFNotifier.Error(string.Format("Document can't be printed because of printer issue : {1}.{0}Administration has been notified about this issue.", Environment.NewLine, e.Issue.Issue()));
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Starts controller logic
        /// </summary>
        public void Start()
        {
            LogHelper.LogDebug();
            StartWatcher();
            StartMonitor();
        }

        /// <summary>
        /// Stops controller logic
        /// </summary>
        public void Stop()
        {
            LogHelper.LogDebug();
            StopWatcher();
            StopMonitor();
        }

        /// <summary>
        /// Sets default printer
        /// </summary>
        public void SetDefaultPrinter()
        {
            try
            {
                // reset last printer used in chrome
                ChromeHelper.ResetLastPrinterUsed();

                // set default printer via Win API
                PrintHelper.SetDefaultPrinter(ConfigData.PrinterName);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        /// <summary>
        /// Shows advanced printer settings
        /// </summary>
        /// <param name="handle">window handle</param>
        /// <param name="printerName">printer name</param>
        /// <param name="settings">settings</param>
        public void ShowAdvancedPrinterSettings(IntPtr handle, string printerName, ref System.Drawing.Printing.PrinterSettings settings)
        {
            // check the printer settings
            if (settings == null || !settings.IsValid)
            {
                LogHelper.LogDebug("Invalid printer settings for " + printerName);
                return;
            }

            LogHelper.LogDebug();

            try
            {
                // create new printer settings
                System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();

                // set new settings as an old one
                ps = settings;

                // show printer settings window as dialog for the current window
                if (PrintDialogHelper.EditPrinterSettings(ps, handle) == System.Windows.Forms.DialogResult.OK)
                {
                    LogHelper.LogDebug("Printer settings changed");

                    // save changed printer settings
                    settings = ps;
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        /// <summary>
        /// Updates available printers
        /// </summary>
        /// <returns>available printers</returns>
        public Printers UpdateAvailablePrinters()
        {
            LogHelper.LogDebug();

            try
            {
                _availablePrinters = null;
                // just retrieve new data
                var xxx = AvailablePrinters;

                LogHelper.LogDebug("HasAvailablePrinters : " + HasAvailablePrinters);

                // change (real) printers of local print event watcher
                if (localPrintEventWatcher != null)
                {
                    localPrintEventWatcher.PrintersToPauseAndShow = new List<string>();

                    // check if there are available printers
                    if (HasAvailablePrinters)
                    {
                        LogHelper.LogDebug("AvailablePrinters : " + AvailablePrinters.NamesInSystem.Count);

                        // add (real) printers
                        localPrintEventWatcher.PrintersToPauseAndShow.AddRange(AvailablePrinters.NamesInSystem);
                    }
                }

                return _availablePrinters;
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        /// <summary>
        /// Launches print control window for the specified file. (Virtual printer method)
        /// </summary>
        /// <param name="eFileName">file name</param>
        /// <param name="copyFile">copy file flag</param>
        public void LaunchPrintControl(string eFileName, bool copyFile = true)
        {
            // check the file name
            if (string.IsNullOrWhiteSpace(eFileName))
                return;

            LogHelper.LogDebug("Launch UI For File " + eFileName);

            try
            {
                string newFileName = eFileName;
                if (copyFile)
                {
                    // create new file name
                    newFileName = Path.Combine(ConfigData.Path_Processing, Path.GetFileName(eFileName));

                    // if the file with new file name exists
                    if (File.Exists(newFileName))
                        // delete the file
                        File.Delete(newFileName);

                    // copy old file to the new one
                    File.Copy(eFileName, newFileName);
                }

                _dispatcher.BeginInvoke(new Action(() =>
                {
                    PrintJobTitle title = null;

                    // check the allowed printers
                    if (localPrintEventWatcher.AllowedPrintersTitles != null && localPrintEventWatcher.AllowedPrintersTitles.Count > 0)
                        // get the last title from the allowed printers (this needs for retrieving the correct print job title, for example)
                        title = localPrintEventWatcher.AllowedPrintersTitles.Last();

                    // set up an launch UI for the incoming file
                    PrintingControlWindow window = new PrintingControlWindow(newFileName, title);
                    window.Closed += WindowClosed;

                    // show UI
                    window.ShowDialog();

                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        /// <summary>
        /// Launches print control window for the specified data. (Real printer method)
        /// </summary>
        /// <param name="printJob"></param>
        public void LaunchPrintControl(PrintJobData printJob)
        {
            // check the print job data
            if (printJob == null || printJob.PrintJobTitle == null)
                return;

            LogHelper.LogDebug("Launch For Real Printer " + printJob.PrintJobTitle);

            try
            {
                _dispatcher.BeginInvoke(new Action(() =>
                {
                    // Update print job logic: 
                    // if the window already created but we received additional data (like number of pages) then find that window and update it's data

                    // iterate through the list of windows
                    foreach (var f in listOfWindows)
                    {
                        LogHelper.LogDebug("Seek Window");

                        // get the data context of the window - has to be a PrintingControlViewModel
                        var dc = f.DataContext as PrintingControlViewModel;

                        // check the data context
                        if (dc != null)
                        {
                            // compare print job titles
                            if (dc.PrintJobTitle.Equals(printJob.PrintJobTitle))
                            {
                                LogHelper.LogDebug("Window found");
                                // update data
                                f.UpdateData(printJob);
                                return;
                            }
                        }
                    }

                    // if the job ain't present for some reason at this moment then leave
                    if (!PrintHelper.HasPrintJob(printJob.ServerHost, printJob.PrintJobTitle))
                        return;

                    // set up an launch UI for the incoming data
                    PrintingControlWindow window = new PrintingControlWindow(printJob);
                    window.Closed += WindowClosed;

                    // add window to a list
                    listOfWindows.Add(window);

                    // show UI
                    window.ShowDialog();

                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }
        
        #endregion

        /// Window Event Handling
        #region Window Event Handling

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void WindowClosed(object sender, EventArgs e)
        {
            try
            {
                // get window as a sender of event
                PrintingControlWindow window = sender as PrintingControlWindow;

                // check the window
                if (window != null && window.ViewModel != null && window.ViewModel.PrintJobTitle != null)
                {
                    // remove window from the list
                    listOfWindows.Remove(window);

                    // remove print job title from the titles
                    localPrintEventWatcher.AllowedPrintersTitles.RemoveTitle(window.ViewModel.PrintJobTitle);

                    // unsubscribe from event
                    window.Closed -= WindowClosed;
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        #endregion

        /// Event Watcher
        #region Event Watcher

        /// <summary>
        /// Starts watcher
        /// </summary>
        private void StartWatcher()
        {
            LogHelper.LogDebug();
            if (localPrintEventWatcher != null)
                localPrintEventWatcher.Start();
        }

        /// <summary>
        /// Stops watcher
        /// </summary>
        private void StopWatcher()
        {
            LogHelper.LogDebug();
            if (localPrintEventWatcher != null)
                localPrintEventWatcher.Stop();
        }

        /// <summary>
        /// Skips the specified document during tracking print jobs
        /// </summary>
        /// <param name="title"></param>
        public void SkipDocument(PrintJobTitle title)
        {
            // check title
            if (title == null)
                return;

            LogHelper.LogDebug(title.MegaToString());

            // check local watcher
            if (localPrintEventWatcher != null)
                // skip document
                localPrintEventWatcher.SkipDocument(title);
        }

        /// <summary>
        /// Adds watcher to the specified print job data
        /// </summary>
        /// <param name="data"></param>
        public void AddWatcher(PrintJobData data)
        {
            // check data
            if (data == null)
                return;

            // check title
            if (data.PrintJobTitle == null)
                return;

            LogHelper.LogDebug(data.PrintJobTitle.ToString());

            // check watchers
            if (_watchers != null)
                // add watcher for the specified title
                _watchers.AddWatcher(new PrintJobWatcher(data.PrintJobTitle));
        }

        /// <summary>
        /// Adds watcher to the specified print job data
        /// </summary>
        /// <param name="data"></param>
        public void AddWatcher(PrintJobTitle data)
        {
            // check title
            if (data == null)
                return;

            // check watchers
            LogHelper.LogDebug(data.ToString());

            if (_watchers != null)
                // add watcher for the specified title
                _watchers.AddWatcher(new PrintJobWatcher(data));
        }

        #endregion

        /// FileMonitor
        #region FileMonitor

        /// <summary>
        /// Starts file monitor
        /// </summary>
        private void StartMonitor()
        {
            LogHelper.LogDebug();
            if (fileMonitor != null)
                fileMonitor.StartMonitor(ConfigData.Path_Monitor);
        }

        /// <summary>
        /// Stops file monitor
        /// </summary>
        private void StopMonitor()
        {
            LogHelper.LogDebug();
            if (fileMonitor != null)
                fileMonitor.StopMonitor();
        }

        #endregion

        /// Dispose
        #region Dispose

        /// <summary>
        /// Disposes data
        /// </summary>
        public void Dispose()
        {
            // stop controllers logic
            Stop();

            try
            {
                // check local watcher
                if (localPrintEventWatcher != null)
                {
                    localPrintEventWatcher.Stop();
                    localPrintEventWatcher.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        #endregion
    }
}
