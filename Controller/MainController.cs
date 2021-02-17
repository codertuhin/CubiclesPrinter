using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows.Forms;
using Cubicles.API;
using Cubicles.API.JsonClasses;
using Cubicles.Events;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinter.View;
using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;
using CubiclesPrinterLib.Utility;
using Timer = System.Timers.Timer;

namespace CubiclesPrinter.Controller
{
    /// <summary>
    /// This class serves as the controller of the application's main logic
    /// </summary>
    public class MainController : IDisposable
    {
        /// Private Variables
        #region Private Variables

        /// <summary> File Monitor </summary>
        IOMonitorHelper fileMonitor = new IOMonitorHelper();

        /// <summary> Watches for print events </summary>
        private PrintEventWatcher localWatcher;

        /// <summary> Watches for print events </summary>
        private PrintEventWatcher serverWatcher;

        /// <summary> Singleton </summary>
        private static MainController _singleton = new MainController();

        /// <summary> List of available printers </summary>
        private Printers _availablePrinters = null;

        /// <summary> Print job </summary>
        private PrintJobData _printJob = null;

        /// <summary> Print job watchers </summary>
        private PrintJobWatchers _watchers = new PrintJobWatchers();

        /// <summary> Retry timer </summary>
        private Timer _timerRetry;

        /// <summary> GetPrinters timer </summary>
        private Timer _timerGetPrinters;

        #endregion

        /// Properties
        #region Properties

        /// <summary> Singleton </summary>
        public static MainController Singleton { get { return _singleton; }}

        /// <summary> MainForm </summary>
        public Form MainForm { get; set; }

        /// <summary> Has Available Printers </summary>
        public bool HasAvailablePrinters { get { return _availablePrinters == null ? false : _availablePrinters.Count < 1 ? false : true;  }}

        /// <summary> List of available printers </summary>
        public Printers AvailablePrinters
        {
            get
            {
                try
                {
                    LogHelper.LogDebug("Seeking printers");
                    if (_availablePrinters == null || _availablePrinters.Count < 1)
                    {
                        if (ConfigData.Config_UseSystemPrinters)
                            _availablePrinters = PrintHelper.GetPrinters();
                        else
                            _availablePrinters = APIWrapper.GetPrinters(Environment.MachineName);
                    }

                    if (_availablePrinters == null || _availablePrinters.Count < 1)
                    {
                        if (ConfigData.Config_UseSystemPrinters)
                            LogHelper.LogDebug("No system printers found");
                        else
                            LogHelper.LogDebug("No printers found : Server ain't responding");

                        string path = ConfigData.FilePath_PrinterConfig;
                        LogHelper.LogDebug("ExePath : " + Application.ExecutablePath);
                        if (string.IsNullOrWhiteSpace(Path.GetPathRoot(ConfigData.FilePath_PrinterConfig)))
                            path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), ConfigData.FilePath_PrinterConfig);

                        _availablePrinters = PrintHelper.GetPrintersFromConfig(path);
                    }

                    LogHelper.LogDebug("Seeking printers : " + (_availablePrinters == null ? "no printers" : _availablePrinters.Count.ToString()));
                }
                catch (Exception ex)
                {
                    Notifier.Error(ex);
                }

                if (_availablePrinters == null || _availablePrinters.Count < 1)
                {
                    LogHelper.LogDebug("No printers found");
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
                Notifier.Error(ex);
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
                Notifier.Error(ex);
            }
        }

        /// <summary>
        /// Removes related jobs
        /// </summary>
        public void RemoveJobs()
        {
            var xxx = AvailablePrinters;
            RemoveJobs1(Environment.MachineName);
            //if (Environment.MachineName != "S77")
                //RemoveJobs1("ADSERVER");
        }

        /// <summary>
        /// Removes related jobs from the specified host
        /// </summary>
        /// <param name="host">host</param>
        private void RemoveJobs1(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
                return;

            if (AvailablePrinters == null || AvailablePrinters.Count < 1)
                return;

            foreach (Printer printer in AvailablePrinters)
                PrintHelper.CancelPrintJobsByUser(host, Environment.MachineName, printer.Name, Environment.UserName);
        }

        /// <summary>
        /// Initializes data
        /// </summary>
        private void Init()
        {
            try
            {
                LogHelper.LogDebug();

                
                

                // load config files
                API.LoadConfig(ConfigData.FilePath_InnerConfig); // local config or 1st config
                API.LoadConfig(ConfigData.FilePath_RemoteConfig); // remote config or 2nd config

                RemoveJobs();

                // establish call-backs
                //PrintHelper.SkipMethod = SkipDocument;

                PrintHelper.SkipMethod = SkipDocument;
                PrintHelper.AddWatcher = AddWatcherX;

                // subscribe to the file monitor event
                fileMonitor.FileFound += fileMonitor_FileFound;

                // (virtual) printers to be excluded from the track process
                List<string> allp = new List<string> { ConfigData.PrinterName, ConfigData.PrinterName2 };

                // init local watcher
                //if (Environment.MachineName == "S77")
                    //localWatcher = new PrintEventWatcher(Environment.MachineName, Environment.MachineName, false, allp);
                //else
                    //localWatcher = new PrintEventWatcher("ADSERVER", Environment.MachineName, false, allp);
                localWatcher = new PrintEventWatcher(Environment.MachineName, Environment.MachineName, false, allp, "0.000000001");
                //localWatcher = new PrintEventWatcher("W10", Environment.MachineName, false, allp);
                localWatcher.PrintJobPaused += LocalWatcherPrintJobPaused;   
                localWatcher.PrintJobStarted += LocalWatcherPrintJobStarted;
                localWatcher.PrintJobCompleted += LocalWatcherPrintJobCompleted;
                localWatcher.PrintJobCancelled += LocalWatcherPrintJobCancelled;
                localWatcher.Init();

                /*
                if (Environment.MachineName != "S77")
                {
                    serverWatcher = new PrintEventWatcher("ADSERVER", Environment.MachineName, true, new List<string>(), "0.00001");
                    serverWatcher.PrintJobPaused += LocalWatcherPrintJobPaused;
                    serverWatcher.PrintJobStarted += LocalWatcherPrintJobStarted;
                    serverWatcher.PrintJobCompleted += LocalWatcherPrintJobCompleted;
                    serverWatcher.PrintJobCancelled += LocalWatcherPrintJobCancelled;
                    serverWatcher.Init();
                }*/

                // update available printers
                UpdateAvailablePrinters();

                // init print job watchers
                _watchers = new PrintJobWatchers();
                _watchers.IssueFound += _watchers_IssueFound;
                _watchers.JobCompleted += _watchers_JobCompleted;
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
                _timerRetry.Start();
                //Environment.Exit(-1);
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
        void LocalWatcherPrintJobPaused(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug();
            LaunchPrintControl(e.Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalWatcherPrintJobCompleted(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug();
            if (e != null && e.Data != null)
            {
                if (_watchers != null && _watchers.Count > 0)
                    _watchers.RemoveWatcher(e.Data);
            }
        }

        private bool cancelledShowing = false;

        void LocalWatcherPrintJobCancelled(object sender, PrintJobDataEventArgs e)
        {
            LogHelper.LogDebug("Cancelled 2! " + e.Data.PrintJobTitle.MegaToString());
            if (_watchers != null && _watchers.Count > 0)
                _watchers.RemoveWatcher(e.Data);

            if (!cancelledShowing)
            {
                cancelledShowing = true;
                Notifier.Warning(string.Format("You are not allowed to choose this printer.{0}You have to use {1} or {2} instead.{0}Printing cancelled.", Environment.NewLine, ConfigData.PrinterName, ConfigData.PrinterName2));
                cancelledShowing = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalWatcherPrintJobStarted(object sender, PrintJobDataEventArgs e)
        {
            /*
            LogHelper.LogDebug();
            try
            {
                if (e != null)
                {
                    if (e.Data != null)
                    {
                        if (e.Data.PrintJobTitle != null)
                        {
                            _watchers.AddWatcher(new PrintJobWatcher(e.Data.PrintJobTitle));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }*/
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
            LaunchPrintControl(e.Data as string);
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

            _timerRetry.Stop();

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

            _timerGetPrinters.Stop();

            if (_availablePrinters == null || _availablePrinters.Count < 1)
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
                if (e != null)
                {
                    PrinterTrouble pt = e.Issue;
                    LogHelper.LogDebug(pt.Issue());
                    if (pt != PrinterTrouble.None)
                    {
                        APIWrapper.PrinterIssue(e.PrinterName, pt);
                        Notifier.Error(string.Format("Document can't be printed because of printer issue : {1}.{0}Administration has been notified about this issue.", Environment.NewLine, pt.Issue()));
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(e.Status))
                        {
                            //Notifier.Error(string.Format("Document can't be printed because of printer issue : {1}", Environment.NewLine, e.Status));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
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
        /// <param name="owner"></param>
        /// <param name="printerName"></param>
        public void ShowAdvancedPrinterSettings(Form owner, string printerName, ref PrinterSettings settings)
        {
            if (settings == null || !settings.IsValid)
            {
                LogHelper.LogDebug("Invalid printer settings for " + printerName);
                return;
            }

            LogHelper.LogDebug();

            try
            {
                PrinterSettings ps = new PrinterSettings();
                ps = settings;

                // show printer settings
                if (PrintDialogHelper.EditPrinterSettings(ps, owner.Handle) == DialogResult.OK)
                {
                    // save printer settings
                    LogHelper.LogDebug("Printer settings changed");
                    settings = ps;
                }
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
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

                // change printers of watcher
                if (localWatcher != null)
                {
                    localWatcher.AllowedPrintersToSkip = new List<string>();
                    localWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName);
                    localWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName2);
                    localWatcher.PrintersToPauseAndShow = new List<string>();

                    LogHelper.LogDebug("HasAvailablePrinters : " + HasAvailablePrinters);

                    if (HasAvailablePrinters)
                    {
                        LogHelper.LogDebug("AvailablePrinters : " + AvailablePrinters.NamesInSystem.Count);
                        localWatcher.PrintersToPauseAndShow.AddRange(AvailablePrinters.NamesInSystem);
                    }
                }

                
                if (serverWatcher != null)
                {
                    serverWatcher.AllowedPrintersToSkip = new List<string>();
                    //serverWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName);
                    //serverWatcher.AllowedPrintersToSkip.Add(ConfigData.PrinterName2);
                    serverWatcher.PrintersToPauseAndShow = new List<string>();

                    if (HasAvailablePrinters)
                    {
                        serverWatcher.PrintersToPauseAndShow.AddRange(AvailablePrinters.NamesInSystemWithoutServer);
                    }
                }

                return _availablePrinters;
            }
            catch (Exception ex)
            {
                Notifier.DebugError(ex);
                return null;
            }
        }

        void form_Closed2(object sender, EventArgs e)
        {
            try
            {
                PrintingControlForm pcf = sender as PrintingControlForm;
                if (pcf != null && pcf.PrintJobTitle != null)
                {
                    localWatcher.AllowedPrintersTitles.RemoveTitle(pcf.PrintJobTitle);
                    pcf.Closed -= form_Closed2;
                    pcf = pcf;
                }
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }
        }

        /// <summary>
        /// Launches print control window for the specified file
        /// </summary>
        /// <param name="eFileName"></param>
        public void LaunchPrintControl(string eFileName, bool copyFile = true)
        {
            LogHelper.LogDebug("Launch Form File " + eFileName);

            try
            {
                string newFileName = eFileName;
                if (copyFile)
                {
                    newFileName = Path.Combine(ConfigData.Path_Processing, Path.GetFileName(eFileName));
                    if (File.Exists(newFileName))
                        File.Delete(newFileName);

                    File.Copy(eFileName, newFileName);
                    //File.Delete(eFileName);
                }

                if (localWatcher.AllowedPrintersTitles.Count < 1)
                    localWatcher.AllowedPrintersTitles.Add(new PrintJobTitle());

                MainForm.BeginInvoke((MethodInvoker) delegate
                {
                    PrintingControlForm form = new PrintingControlForm(newFileName, localWatcher.AllowedPrintersTitles.Last());
                    form.Closed += form_Closed2;
                    form.ShowDialog(MainForm);
                });
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }
        }

        public void LaunchPrintControl(PrintJobData printJob)
        {
            LogHelper.LogDebug("Launch Form " + printJob.PrintJobTitle);
            _printJob = printJob;
            /*
            try
            {
                MainForm.BeginInvoke((MethodInvoker)delegate
                {
                    foreach (var f in listF)
                    {
                        LogHelper.LogDebug("Seek Form");
                        if (f.PrintJobData.PrintJobTitle.Equals(printJob.PrintJobTitle))
                        {
                            LogHelper.LogDebug("Form found");
                            f.UpdateData(printJob);
                            return;
                        }
                    }

                    if (!PrintHelper.HasPrintJob(printJob.ServerHost, printJob.PrintJobTitle))
                        return;

                    PrintingControlForm form = new PrintingControlForm(_printJob);
                    listF.Add(form);
                    form.Closed += form_Closed;
                    form.ShowDialog(MainForm);
                });
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }*/
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
            if (localWatcher != null)
                localWatcher.Start();
            
            if (serverWatcher != null)
                serverWatcher.Start();
        }

        /// <summary>
        /// Stops watcher
        /// </summary>
        private void StopWatcher()
        {
            LogHelper.LogDebug();
            if (localWatcher != null)
                localWatcher.Stop();
            
            if (serverWatcher != null)
                serverWatcher.Stop();
        }

        /// <summary>
        /// Skips the specified document during tracking print jobs
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="documentName"></param>
        void SkipDocument(string printerName, string documentName, string owner, string host, int jobID)
        {
            LogHelper.LogDebug(printerName + " " + documentName);
            if (localWatcher != null)
                localWatcher.SkipDocument(printerName, documentName, owner, host, jobID);
            
            //if (serverWatcher != null)
                //serverWatcher.SkipDocument(printerName.Replace("\\\\ADSERVER\\", ""), documentName, owner, host, jobID);
        }

        public void SkipDocument(PrintJobTitle title)
        {
            SkipDocument(title.PrinterName, title.Document, title.Owner, title.Host, title.JobID);
        }

        /// <summary>
        /// Adds watcher to the specified print job data
        /// </summary>
        /// <param name="data"></param>
        public void AddWatcher(PrintJobData data)
        {
            if (data == null)
                return;

            if (data.PrintJobTitle == null)
                return;

            LogHelper.LogDebug(data.PrintJobTitle.ToString());

            if (_watchers != null)
                _watchers.AddWatcher(new PrintJobWatcher(data.PrintJobTitle));
        }

        public void AddWatcherX(PrintJobTitle data)
        {
            if (data == null)
                return;

            LogHelper.LogDebug(data.ToString());

            if (_watchers != null)
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
            Stop();

            try
            {
                if (localWatcher != null)
                {
                    localWatcher.Stop();
                    localWatcher.Dispose();
                }

                
                if (serverWatcher != null)
                {
                    serverWatcher.Stop();
                    serverWatcher.Dispose();
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
