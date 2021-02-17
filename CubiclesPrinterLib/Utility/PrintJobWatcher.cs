using System;
using System.Collections.Generic;
using System.Timers;
using Cubicles.API.JsonClasses;
using Cubicles.Utility;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;

namespace CubiclesPrinterLib.Utility
{
    /// <summary>
    /// This class represents print job watcher
    /// </summary>
    public class PrintJobWatcher : IDisposable
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Print job </summary>
        //private PrintJobData _printJob = null;

        /// <summary> Print Job Title </summary>
        private PrintJobTitle PrintJobTitle = null;

        /// <summary> Printer status </summary>
        private PrinterTrouble _printerStatus;

        /// <summary> Timer </summary>
        private Timer timer;
        
        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printJob"></param>
        public PrintJobWatcher(PrintJobTitle printJobTitle)
        {
            if (printJobTitle == null)
                return;
            /*
            if (printJob == null)
                return;

            if (printJob.PrintJob == null)
                return;*/

            PrintJobTitle = printJobTitle;
            _printerStatus = PrinterTrouble.None;

            // initialize timer
            timer = new Timer();
            timer.Enabled = false;
            timer.Interval = ConfigData.Interval_TimerPrintJobWatcher;
            timer.Elapsed += timer_Elapsed;
        }
        
        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Checks if the watcher is watching specified job
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsWatchingThisJob(PrintJobData data)
        {
            if (data == null)
                return false;

            //if (_printJob.PrintJobTitle.PrinterName == data.PrintJobTitle.PrinterName && _printJob.PrintJobTitle.Document == data.PrintJobTitle.Document)
            if (PrintJobTitle.Equals(data.PrintJobTitle))
                return true;

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerCall();
        }

        /// <summary>
        /// Starts timer
        /// </summary>
        public void Start()
        {
            if (timer == null)
                return;

            timer.Start();
        }

        /// <summary>
        /// Stops timer
        /// </summary>
        public void Stop()
        {
            if (timer == null)
                return;

            timer.Stop();
        }

        /// <summary>
        /// Checks print job and printer status
        /// </summary>
        private void TimerCall()
        {
            timer.Enabled = false;

            bool jobfound = false;
            bool hasTrouble = false;

            try
            {
                string statusReport = "";
                //_printerStatus |= PrintHelper.GetPrinterStatus(_printJob.ServerHost, _printJob.PrintJobTitle.PrinterName, out statusReport);
                _printerStatus |= PrintHelper.GetPrinterStatus(PrintJobTitle.Host, PrintJobTitle.PrinterName, out statusReport);

                if ((_printerStatus | PrinterTrouble.None) == PrinterTrouble.None)
                {
                    hasTrouble = false;
                }
                else
                {
                    hasTrouble = true;
                }

                // if issue found
                if (hasTrouble)
                {
                    // send data to server
                    //OnIssueFound(new PrinterIssueEventArgs(_printJob.PrintJobTitle.PrinterName, _printerStatus, statusReport));
                    OnIssueFound(new PrinterIssueEventArgs(PrintJobTitle.PrinterName, _printerStatus, statusReport));
                    OnCanBeDisposed(EventArgs.Empty);
                    return;
                }

                // find the job
                //foreach (var printJob in PrintHelper.GetPrintJobs(_printJob.ServerHost, _printJob.PrintJobTitle.PrinterName))
                foreach (var printJob in PrintHelper.GetPrintJobs(PrintJobTitle.Host, PrintJobTitle.PrinterName))
                {
                    //if (PrintHelper.ExtractDocumentName(printJob) == _printJob.PrintJobTitle.Document)
                    if (PrintHelper.ExtractDocumentName(printJob) == PrintJobTitle.Document)
                    {
                        jobfound = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }

            // if job found
            if (jobfound)
            {
                // continue watching
                timer.Enabled = true;
            }
            else
            {
                // finish watching
                OnJobCompleted(new PrintJobDataEventArgs(null));
                OnCanBeDisposed(EventArgs.Empty);
            }
        }
        
        #endregion

        /// Dispose
        #region Dispose

        /// <summary>
        /// Disposes the object
        /// </summary>
        public void Dispose()
        {
            Stop();
        }

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<EventArgs> CanBeDisposed;

        private void OnCanBeDisposed(EventArgs e)
        {
            EventHandler<EventArgs> handler = CanBeDisposed;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PrinterIssueEventArgs> IssueFound;

        private void OnIssueFound(PrinterIssueEventArgs e)
        {
            EventHandler<PrinterIssueEventArgs> handler = IssueFound;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PrintJobDataEventArgs> JobCompleted;

        private void OnJobCompleted(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = JobCompleted;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }

    /// <summary>
    /// The list of PrintJobWatcher objects
    /// </summary>
    public class PrintJobWatchers : List<PrintJobWatcher>
    {
        /// Methods
        #region Methods

        /// <summary>
        /// Adds watcher
        /// </summary>
        /// <param name="watcher"></param>
        public void AddWatcher(PrintJobWatcher watcher)
        {
            if (watcher == null)
                return;

            watcher.CanBeDisposed += watcher_CanBeDisposed;
            watcher.IssueFound += watcher_IssueFound;
            watcher.JobCompleted += watcher_JobCompleted;

            watcher.Start();

            Add(watcher);
        }

        /// <summary>
        /// Removes watcher
        /// </summary>
        /// <param name="data"></param>
        public void RemoveWatcher(PrintJobData data)
        {
            if (data == null)
                return;

            PrintJobWatcher pjw = null;
            foreach (var watcher in this)
            {
                if (watcher != null)
                {
                    if (watcher.IsWatchingThisJob(data))
                    {
                        pjw = watcher;
                        return;
                    }
                }
            }

            watcher_CanBeDisposed(pjw, EventArgs.Empty);
        }

        #endregion

        /// Event Handlers
        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_JobCompleted(object sender, PrintJobDataEventArgs e)
        {
            OnJobCompleted(e);
        }

        void watcher_IssueFound(object sender, PrinterIssueEventArgs e)
        {
            OnIssueFound(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_CanBeDisposed(object sender, EventArgs e)
        {
            PrintJobWatcher w = sender as PrintJobWatcher;
            if (w != null)
            {
                try
                {
                    this.Remove(w);
                    w.Dispose();
                }
                catch (Exception ex)
                {
                    WPFNotifier.DebugError(ex);
                }
            }
        }

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PrinterIssueEventArgs> IssueFound;

        private void OnIssueFound(PrinterIssueEventArgs e)
        {
            EventHandler<PrinterIssueEventArgs> handler = IssueFound;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PrintJobDataEventArgs> JobCompleted;

        private void OnJobCompleted(PrintJobDataEventArgs e)
        {
            EventHandler<PrintJobDataEventArgs> handler = JobCompleted;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
