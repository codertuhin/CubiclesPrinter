using System;
using System.Collections.Generic;
using System.Management;
using System.Threading;
using System.Timers;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;

namespace CubiclesPrinterLib.Utility
{
    /// <summary>
    /// This class allows to watch for print events
    /// </summary>
    public class PrintEventWatcher : IDisposable
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Printer event watcher </summary>
        private ManagementEventWatcher _eventWatcher;

        /// <summary> Management scope </summary>
        private ManagementScope managementScope;

        /// <summary> Printer host </summary>
        private string _host;

        /// <summary> Printer user </summary>
        private string _user = Environment.UserName;

        /// <summary> Printer user </summary>
        private string _jobHost = Environment.UserName;

        private string _interval = "0.0001";

        /// <summary> Printers to be skipped during overall watch process </summary>
        private List<string> _allowedPrintersToSkip;

        /// <summary> Printers to be paused during overall watch process and the app screen to be showed </summary>
        private List<string> _printersToPauseAndShow;

        /// <summary> Titles to skip </summary>
        private PrintJobTitles _titlesToSkip;

        /// <summary>  </summary>
        private PrintJobTitles _allowedPrintersTitles;
        
        /// <summary> Sets up watcher activity flag </summary>
        private bool _doWatching = false;

        /// <summary> </summary>
        private PrintJobTitles _titlesProcessed = new PrintJobTitles(); 

        #endregion

        /// Properties
        #region Properties

        /// <summary> Printers to be skipped during overall watch process </summary>
        public List<string> AllowedPrintersToSkip { get { return _allowedPrintersToSkip; } set { _allowedPrintersToSkip = value; } }

        /// <summary> Printers to be skipped during overall watch process </summary>
        public PrintJobTitles AllowedPrintersTitles { get { return _allowedPrintersTitles; } set { _allowedPrintersTitles = value; } }

        /// <summary> Printers to be paused during overall watch process and the app screen to be showed </summary>
        public List<string> PrintersToPauseAndShow { get { return _printersToPauseAndShow; } set { _printersToPauseAndShow = value; } }

        #endregion

        /// Init
        #region Init
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">host name where to inspect printers</param>
        PrintEventWatcher(string host, string jobHost, bool remote, string interval)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException("No host specified");

            _titlesToSkip = new PrintJobTitles();

            _host = host;
            _jobHost = jobHost;

            LogHelper.LogDebug(_host + " " + _jobHost);

            _allowedPrintersToSkip = new List<string>();
            _allowedPrintersTitles = new PrintJobTitles();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">printer host</param>
        /// <param name="allowedPrinters">printers to be skipped during overall watch process</param>
        public PrintEventWatcher(string host, string jobHost, bool remote, List<string> allowedPrinters, string interval)
            : this(host, jobHost, remote, interval)
        {
            if (allowedPrinters != null && allowedPrinters.Count > 0)
                _allowedPrintersToSkip.AddRange(allowedPrinters);
        }

        /// <summary>
        /// Initializes data
        /// </summary>
        public void Init()
        {
            LogHelper.LogDebug(_host);

            try
            {
                // set connection options
                ConnectionOptions co = new ConnectionOptions();
                co.EnablePrivileges = true;
                co.Authentication = AuthenticationLevel.Default;
                co.Impersonation = ImpersonationLevel.Impersonate;
                
                // initialize __InstanceCreationEvent
                EventWatcherOptions ewo = new EventWatcherOptions();

                // subscribe to InstanceOperationEvent
                _eventWatcher = new ManagementEventWatcher();
                _eventWatcher.Query = new EventQuery("SELECT * FROM __InstanceOperationEvent WITHIN " + _interval + " WHERE TargetInstance ISA \"Win32_PrintJob\"");
                _eventWatcher.Scope = new ManagementScope(@"\\" + _host + @"\root\CIMV2", co);
                _eventWatcher.Options = ewo;
                _eventWatcher.EventArrived += mewPrintJobs_OperationSet;

                // start the watcher
                _eventWatcher.Start();

                LogHelper.LogDebug("Active " + _host);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PrintJobDataEventArgs> PrintJobStarted;

        private void OnPrintJobStarted(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = PrintJobStarted;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<PrintJobDataEventArgs> PrintJobPaused;

        private void OnPrintJobPaused(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = PrintJobPaused;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<PrintJobDataEventArgs> PrintJobCancelled;

        private void OnPrintJobCancelled(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = PrintJobCancelled;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<PrintJobDataEventArgs> PrintJobCompleted;

        private void OnPrintJobCompleted(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = PrintJobCompleted;
            if (handler != null)
                handler(this, e);
        }
        
        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Starts watching
        /// </summary>
        public void Start()
        {
            _doWatching = true;
        }

        /// <summary>
        /// Stops watching
        /// </summary>
        public void Stop()
        {
            _doWatching = false;
        }

        /// <summary>
        /// Skips specified title during watch process
        /// </summary>
        /// <param name="title">title to be skipped during watch process [during document print]</param>
        public void SkipDocument(PrintJobTitle title)
        {
            // check title
            if (title == null)
                return;

            lock (_titlesToSkip)
            {
                LogHelper.Log("SkipDocument : " + title.ToString());
                _titlesToSkip.Add(title);
            }
        }

        /// <summary>
        /// Resets skip data
        /// </summary>
        /// <param name="data">data to be clear</param>
        public void ResetSkip(PrintJobData data)
        {
            if (data == null)
                return;

            if (data.PrintJobTitle == null)
                return;

            LogHelper.Log("ResetSkip : " + data.PrintJobTitle.ToString());
            ResetSkip(data.PrintJobTitle.PrinterName, data.PrintJobTitle.Document, data.PrintJobTitle.Owner, data.PrintJobTitle.Host, data.PrintJobTitle.JobID);
        }

        /// <summary>
        /// Resets skip data
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="documentName"></param>
        public void ResetSkip(string printerName, string documentName, string owner, string host, int jobID)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return;

            if (string.IsNullOrWhiteSpace(documentName))
                return;

            if (string.IsNullOrWhiteSpace(owner))
                return;

            if (string.IsNullOrWhiteSpace(host))
                return;
            
            lock (_titlesToSkip)
            {
                LogHelper.LogDebug(_host + " : " + printerName + " | " + documentName);
                _titlesToSkip.RemoveTitle(printerName, documentName, owner, host, jobID);
            }
        }

        /// <summary>
        /// Check if it's the same printer
        /// </summary>
        /// <param name="currentPrinterName"></param>
        /// <param name="comparaPrinterName"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        private bool IsSamePrinter(string currentPrinterName, string comparaPrinterName, string host)
        {
            LogHelper.LogDebug(_host + " : " + currentPrinterName + " : " + comparaPrinterName + " : " + host);

            if (string.IsNullOrWhiteSpace(currentPrinterName))
                return false;

            if (string.IsNullOrWhiteSpace(comparaPrinterName))
                return false;

            if (currentPrinterName == comparaPrinterName)
                return true;

            if (string.IsNullOrWhiteSpace(host))
                host = "";

            if (comparaPrinterName.Contains(currentPrinterName))
                return true;

            if (currentPrinterName.Contains(comparaPrinterName))
                return true;

            if (currentPrinterName == comparaPrinterName.Replace(@"\\" + _jobHost + @"\", ""))
                return true;

            if (comparaPrinterName == currentPrinterName.Replace(@"\\" + _jobHost + @"\", ""))
                return true;

            if (currentPrinterName == comparaPrinterName.Replace(@"\\" + _host + @"\", ""))
                return true;

            if (comparaPrinterName == currentPrinterName.Replace(@"\\" + _host + @"\", ""))
                return true;

            LogHelper.LogDebug(_host + " : " + "Not same");
            return false;
        }


        public event EventHandler<PrintJobDataEventArgs> AllowedTitleAdded;

        private void OnAllowedTitleAdded(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = AllowedTitleAdded;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJobData"></param>
        /// <param name="raise"></param>
        /// <returns></returns>
        private bool CanSkip(PrintJobData printJobData, bool raise = false)
        {
            // check printer of the job
            foreach (var _allowedPrinterToSkip in _allowedPrintersToSkip)
            {
                if (_allowedPrinterToSkip == printJobData.PrintJobTitle.PrinterName)
                {
                    lock (_allowedPrintersTitles)
                    {
                        if (!_allowedPrintersTitles.HasTitle(printJobData))
                        {
                            _allowedPrintersTitles.Add(printJobData);
                            OnAllowedTitleAdded(new PrintJobDataEventArgs(printJobData));                           

                            /*
                            while (true)
                            {
                                var jobInfo2 = CubiclesPrinterLib.Win32.Print.PrintJob.GetJobInfo(printJobData.RemotePrinterName, printJobData.PrintJobTitle.JobID);
                                if (jobInfo2 != null)
                                    printJobData.NumberOfCopies = jobInfo2.dmOut.dmCopies;
                            }*/
                        }
                    }

                    return true;
                }
            }

            LogHelper.LogDebug(printJobData.PrintJobTitle.Host + " | " + printJobData.PrintJobTitle.Owner + " | raise : " + raise);

            // check the host of the job
            string host = printJobData.PrintJobTitle.Host;
            LogHelper.LogDebug(_host + " : " + "host of the job " + host + " | current host " + _jobHost);
            if (!string.IsNullOrWhiteSpace(host))
                if (host != @"\\" + _jobHost && host != _jobHost && _jobHost != @"\\" + host)
                    return true;

            // check the user of the job
            string user = printJobData.PrintJobTitle.Owner;
            LogHelper.LogDebug(_host + " : " + "user of the job " + user + " | current user " + _user);
            if (!string.IsNullOrWhiteSpace(user))
                if (user != _user)
                    return true;

            LogHelper.LogDebug(_host + " : " + "Check " + printJobData.PrintJobTitle.Document + " | " + _titlesToSkip.Count);

            lock (_titlesToSkip)
            {
                if (_titlesToSkip != null && _titlesToSkip.Count > 0)
                {
                    LogHelper.LogDebug(_host + " : " + "Titles");
                    // check the document of the job
                    if (_titlesToSkip.HasTitle(printJobData))
                    {
                        LogHelper.LogDebug(_host + " : " + "Has title " + printJobData.PrintJobTitle.Document);
                        //if (raise)
                            //OnPrintJobStarted(new PrintJobDataEventArgs(printJobData));
                        return true;
                    }
                }
            }

            LogHelper.LogDebug(_host + " : " + "Check Quit");

            return false;
        }

        private bool CanSkip2(PrintJobData printJobData, bool raise = false)
        {
            // check printer of the job
            foreach (var _allowedPrinterToSkip in _allowedPrintersToSkip)
            {
                if (_allowedPrinterToSkip == printJobData.PrintJobTitle.PrinterName)
                {
                    lock (_allowedPrintersTitles)
                    {
                        if (!_allowedPrintersTitles.HasTitle(printJobData))
                            _allowedPrintersTitles.Add(printJobData);
                    }

                    return true;
                }
            }

            LogHelper.LogDebug(printJobData.PrintJobTitle.Host + " | " + printJobData.PrintJobTitle.Owner + " | raise : " + raise);

            // check the host of the job
            string host = printJobData.PrintJobTitle.Host;
            LogHelper.LogDebug(_host + " : " + "host of the job " + host + " | current host " + _jobHost);
            if (!string.IsNullOrWhiteSpace(host))
                if (host != @"\\" + _jobHost && host != _jobHost && _jobHost != @"\\" + host)
                    return true;

            // check the user of the job
            string user = printJobData.PrintJobTitle.Owner;
            LogHelper.LogDebug(_host + " : " + "user of the job " + user + " | current user " + _user);
            if (!string.IsNullOrWhiteSpace(user))
                if (user != _user)
                    return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJobData"></param>
        /// <param name="raise"></param>
        private void XXX(PrintJobData printJobData, bool raise)
        {
            LogHelper.LogDebug(_host + " : " + "printersToPauseAndShow : " + _printersToPauseAndShow.Count);
            foreach (var _printerToPauseAndShow in _printersToPauseAndShow)
            {
                LogHelper.LogDebug(_printerToPauseAndShow);
                if (IsSamePrinter(printJobData.PrintJobTitle.PrinterName, _printerToPauseAndShow, _jobHost))
                {
                    LogHelper.LogDebug(_host + " : " + "Same printer, pages " + printJobData.PrintJobTitle.TotalPages.ToString());
                    if (printJobData.PrintJobTitle.TotalPages > 0)
                    {
                        lock (_titlesProcessed)
                        {
                            if (!_titlesProcessed.HasTitleAndMorePages(printJobData))
                            {
                                _titlesProcessed.RemoveTitle(printJobData.PrintJobTitle);
                                _titlesProcessed.Add(printJobData.PrintJobTitle);
                                LogHelper.LogDebug(_host + " : " + "titlesProcessed : " + _titlesProcessed.Count);
                                //PrintHelper.PausePrintJob(printJobData.PrintJob, _host);
                                if (raise)
                                    OnPrintJobPaused(new PrintJobDataEventArgs(printJobData));
                            }
                        }
                    }
                    return;
                }
            }
        }

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mewPrintJobs_OperationSet(object sender, System.Management.EventArrivedEventArgs e)
        {
            string eventName = e.NewEvent.ClassPath.ClassName;
            switch (eventName)
            {
                case "__InstanceCreationEvent":
                    mewPrintJobs_JobArrived(sender, e);
                    break;
                case "__InstanceModificationEvent":
                    mewPrintJobs_EventSet(sender, e);
                    break;
                case "__InstanceDeletionEvent":
                    mewPrintJobs_EventDeleted(sender, e);
                    break;
            }
        }

        /// <summary>
        /// Print event arrived
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mewPrintJobs_JobArrived(object sender, EventArrivedEventArgs e)
        {
            if (!_doWatching)
                return;

            try
            {
                // get arrived print job
                ManagementBaseObject printJob = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
                if (printJob == null)
                    return;

                PrintJobData printJobData = new PrintJobData(printJob, _host);

                LogHelper.LogDebug(_host + " : " + printJobData.PrintJobTitle + " | " + printJobData.JobStatus);

                if (CanSkip(printJobData, false))
                {
                    //ResetSkip(printJobData);
                    //OnPrintJobStarted(new PrintJobDataEventArgs(printJobData));
                    return;
                }

                foreach (var _printerToPauseAndShow in _printersToPauseAndShow)
                {
                    if (IsSamePrinter(_printerToPauseAndShow, printJobData.PrintJobTitle.PrinterName, _jobHost))
                    {
                        if (ConfigData.Config_InterceptPrintJobAndCancel)
                        {
                            if (printJobData.PrintJobTitle.TotalPages > 0)
                                LogHelper.LogDebug(_host + " : TotalPages " + printJobData.PrintJobTitle.TotalPages);

                            // this job has to be intercepted
                            //PrintHelper.CancelPrintJob(printJob);
                            PrintHelper.CancelAllPrintJobs(printJob, _host);

                            OnPrintJobCancelled(new PrintJobDataEventArgs(printJobData));
                            //if (_host != "ADSERVER")
                            //WPFNotifier.Warning(string.Format("You are not allowed to choose this printer.{0}You have to use {1} or {2} instead.{0}Printing cancelled.", Environment.NewLine, ConfigData.PrinterName, ConfigData.PrinterName2));
                            return;
                        }
                        else
                        {
                            PrintHelper.PausePrintJob(printJob, _host);
                        }
                    }
                }

                /*
                // this job has to be intercepted
                PrintHelper.CancelPrintJob(printJob);
                WPFNotifier.Warning(string.Format("You are not allowed to choose this printer.{0}You have to use {1} instead.{0}Printing cancelled.", Environment.NewLine, ConfigData.PrinterName));*/
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mewPrintJobs_EventSet(object sender, System.Management.EventArrivedEventArgs e)
        {
            if (!_doWatching)
                return;

            try
            {
                // get arrived print job
                ManagementBaseObject printJob = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
                if (printJob == null)
                    return;

                PrintJobData printJobData = new PrintJobData(printJob, _host);

                LogHelper.LogDebug(_host + " : " + printJobData.PrintJobTitle + " | " + printJobData.JobStatus);

                if (CanSkip(printJobData))
                {
                    if (printJobData.JobStatus.JobStatus.HasFlag(PrintJobStatusEnum.Deleting) || printJobData.JobStatus.JobStatus == (PrintJobStatusEnum.Deleting | PrintJobStatusEnum.Error))
                    {
                        //LogHelper.LogDebug(_host + " : Reset Skip " + printJobData.PrintJobTitle);
                        //ResetSkip(printJobData);
                    }
                    return;
                }

                if (ConfigData.Config_InterceptPrintJobAndCancel)
                {
                    PrintHelper.CancelAllPrintJobs(printJob, _host);
                }
                else
                {
                    if (printJobData.JobStatus.JobStatus == PrintJobStatusEnum.Paused ||
                        printJobData.JobStatus.JobStatus == PrintJobStatusEnum.Printing ||
                        printJobData.JobStatus.JobStatus == (PrintJobStatusEnum.Paused | PrintJobStatusEnum.Printing))
                    {
                        XXX(printJobData, true);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mewPrintJobs_EventDeleted(object sender, System.Management.EventArrivedEventArgs e)
        {
            //if (ConfigData.Config_InterceptPrintJobAndCancel)
                //return;

            if (!_doWatching)
                return;

            try
            {
                // get arrived print job
                ManagementBaseObject printJob = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
                if (printJob == null)
                    return;

                PrintJobData printJobData = new PrintJobData(printJob, _host);

                LogHelper.LogDebug(_host + " : " + printJobData.PrintJobTitle + " | " + printJobData.JobStatus);

                if (CanSkip2(printJobData))
                    return;

                lock (_titlesProcessed)
                    _titlesProcessed.RemoveTitle(printJobData);

                //lock (_allowedPrintersTitles)
                    //_allowedPrintersTitles.RemoveTitle(printJobData);

                ResetSkip(printJobData);

                OnPrintJobCompleted(new PrintJobDataEventArgs(printJobData));
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug(ex);
            }
        }

        #endregion

        /// Dispose
        #region Dispose

        /// <summary>
        /// Disposes data
        /// </summary>
        public void Dispose()
        {
            // get rid of event watcher
            if (_eventWatcher != null)
            {
                try
                {
                    _eventWatcher.EventArrived -= mewPrintJobs_JobArrived;
                    _eventWatcher.Stop();

                    _eventWatcher.Dispose();
                    _eventWatcher = null;
                }
                catch (Exception ex)
                {
                    WPFNotifier.DebugError(ex);
                }
            }

            // get rid of management scope
            if (managementScope != null)
            {
                try
                {
                    managementScope = null;
                }
                catch (Exception ex)
                {
                    WPFNotifier.DebugError(ex);
                }
            }
        }

        #endregion
    }
}
