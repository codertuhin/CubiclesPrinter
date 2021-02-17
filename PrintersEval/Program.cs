using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;
using CubiclesPrinterLib.Utility;
using System;
using System.Collections.Generic;
using System.Management;
using System.Timers;

namespace PrintersEval
{
    class Program
    {
        private static PrintEventWatcher localWatcher;
        private static PrintJobWatchers _watchers;
        private static Timer timer = new Timer();
        private static bool hasIssue;
        private static string printerWithIssue;
        /// <summary> Printer event watcher </summary>
        private static ManagementEventWatcher _eventWatcher;

        static void Main(string[] args)
        { 
            //for testing purposes get the printer from user, in real life it will came from db or command line argument
            string[] printers = new string[] 
            {
                @"\\ADSERVER\BLACK & WHITE $.25 CENTS PER PAGE.",
                @"\\ADSERVER\COLOR PRINTER $.75 CENTS PER PAGE."
            };

            Console.Write("Black and white = 1. color = 2. Choose printer: ");
            string p = Console.ReadLine();
            int pr;
            if(int.TryParse(p, out pr))
            {
                printerWithIssue = printers[pr - 1];
            }
            else
            {
                return;
            }

            //init timer that will send the test prints
            timer.Interval = 6000;
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
            Console.WriteLine("Timer started");
            Console.ReadLine();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer Elapsed");
            timer.Stop();
            InitWatchers();
            localWatcher.Start();
            hasIssue = false;
            Console.WriteLine("before print test page");

            PrintHelper ph = new PrintHelper();
            //ph.PrintTestPage(printerWithIssue);

            Console.WriteLine("print test page done");
        }

        static void InitWatchers()
        {
            Console.WriteLine("Init Watchers start");

            string host = Environment.MachineName;

            //this watcher would pick up on all print events 
            ServerPrintEventWatcher(host);

            // (virtual) printers to be excluded from the track process
            List<string> allp = new List<string> { ConfigData.PrinterName, ConfigData.PrinterName2 };

            // init local watcher
            //localWatcher = new PrintEventWatcher(host, allp);
            localWatcher.PrintJobCompleted += LocalWatcherPrintJobCompleted;
           
            localWatcher.Init();

            Console.WriteLine("local watcher initiolized, _watcher start");

            // init print job watchers
            _watchers = new PrintJobWatchers();
            _watchers.IssueFound += _watchers_IssueFound;
            _watchers.JobCompleted += _watchers_JobCompleted;

            Console.WriteLine("_watcher initiolized");
        }

        private static void LocalWatcherPrintJobCompleted(object sender, PrintJobDataEventArgs e)
        {
            _watchers.RemoveWatcher(e.Data);
            Console.WriteLine("Watcher removed, print job completed");
            localWatcher.Stop();

            if (hasIssue)
            {
                Console.WriteLine("still has an issue, restart timer");
                timer.Start();
            }
            else
            {
                Console.WriteLine("Print job completed without issue, notofy api and exit");
            }

        }

        private static void LocalWatcherPrintJobStarted(object sender, PrintJobDataEventArgs e)
        {
            _watchers.AddWatcher(new PrintJobWatcher(e.Data.PrintJobTitle));
            Console.WriteLine("watcher added, print job started");
        }

        private static void _watchers_JobCompleted(object sender, PrintJobDataEventArgs e)
        {
            Console.WriteLine("Job Completed");
            
        }

        private static void _watchers_IssueFound(object sender, PrinterIssueEventArgs e)
        {
            Console.WriteLine("Issue found {0}", e.Issue);
            hasIssue = true;
        }



        private static void ServerPrintEventWatcher(string host)
        {
            ConnectionOptions co = new ConnectionOptions
            {
                EnablePrivileges = true,
                Authentication = AuthenticationLevel.Default,
                Impersonation = ImpersonationLevel.Impersonate
            };

            // initialize __InstanceCreationEvent
            EventWatcherOptions ewo = new EventWatcherOptions();

            _eventWatcher = new ManagementEventWatcher
            {
                Query = new EventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 0.001 WHERE TargetInstance ISA \"Win32_PrintJob\""),
                Scope = new ManagementScope(@"\\" + host + @"\root\CIMV2", co),
                Options = ewo
            };
            _eventWatcher.EventArrived += NewPrintJobs_JobArrived;
            _eventWatcher.Start();
        }

        private static void NewPrintJobs_JobArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject printJob = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            if (printJob == null)
                return;

            //PrintJobData printJobData = new PrintJobData(printJob);
            //_watchers.AddWatcher(new PrintJobWatcher(printJobData));
            Console.WriteLine("watcher added, print job started");
        }
    }
}
