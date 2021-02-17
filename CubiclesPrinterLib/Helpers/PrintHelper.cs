using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Management;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using Cubicles.API;
using Cubicles.API.JsonClasses;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// PrintHelper class
    /// </summary>
    public class PrintHelper
    {
        /// Private Variables
        #region Private Variables

        /// <summary> search query - print jobs</summary>
        private static string _searchQueryPrintJobs = "SELECT * FROM Win32_PrintJob";

        /// <summary> search query - printers </summary>
        private static string _searchQueryPrinters = "SELECT * FROM Win32_Printer";

        #endregion        

        /// GetPrinters
        #region GetPrinters

        /// <summary>
        /// Gets printer list from the specified config file
        /// </summary>
        /// <param name="fullPath">config file</param>
        /// <returns>printers</returns>
        public static Printers GetPrintersFromConfig(string fullPath)
        {
            LogHelper.LogDebug(fullPath);
            if (String.IsNullOrWhiteSpace(fullPath))
                return null;
            
            try
            {
                if (!File.Exists(fullPath))
                    return null;

                string data = File.ReadAllText(fullPath);
                if (String.IsNullOrWhiteSpace(data))
                    return null;

                LogHelper.LogDebug("From file: " + data);

                JArray jArray = JArray.Parse(data);
                foreach (JObject jObject in jArray.Children<JObject>())
                {
                    var workstation = jObject["workstation"];
                    if (workstation != null)
                    {
                        if (workstation.ToString() == Environment.MachineName)
                        {
                            LogHelper.LogDebug("Printer data found");
                            return JsonConvert.DeserializeObject<Printers>(jObject["printers"].ToString());
                        }
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<Printers>(jArray.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }

            LogHelper.LogDebug("Printer data not found");
            return null;
        }

        /// <summary>
        /// Checks if the print queue of the specified host has the specified title
        /// </summary>
        /// <param name="host">host to be inspected</param>
        /// <param name="title">title to be found</param>
        /// <returns>true if the title is present; otherwise false</returns>
        public static bool HasPrintJob(string host, PrintJobTitle title)
        {
            if (String.IsNullOrWhiteSpace(host))
                return false;

            if (title == null)
                return false;

            try
            {
                foreach (ManagementObject item in GetPrintJobs(host, title.PrinterName))
                {
                    if (title.Equals(new PrintJobTitle(item)))
                        return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static List<ManagementObject> GetPrintJobs(string host, string printerName)
        {
            try
            {
                LogHelper.LogDebug(host);
                ManagementScope scope = new ManagementScope("\\\\" + host + "\\root\\cimv2");
                scope.Connect();
                //LogHelper.LogDebug(host + "1");
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(scope, new ObjectQuery(_searchQueryPrintJobs));
                List<ManagementObject> col = new List<ManagementObject>();
                //LogHelper.LogDebug(host + "2");
                foreach (ManagementObject item in searchPrintJobs.Get())
                {
                    //LogHelper.LogDebug(host + "3");
                    if (ExtractPrinterName(item) == printerName)
                        col.Add(item);
                }

                return col;
            }
            catch (Exception ex)
            {
                LogHelper.Log(String.Format("{1}{0}{2}{0}{3}", Environment.NewLine, host, printerName, ex.ToString()));
                //WPFNotifier.Error(ex, );
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Printers GetPrinters()
        {
            try
            {
                LogHelper.LogDebug();
                Printers printers = new Printers();
                ManagementObjectSearcher searchPrinters = new ManagementObjectSearcher(_searchQueryPrinters);
                ManagementObjectCollection printerCollection = searchPrinters.Get();
                foreach (ManagementObject printer in printerCollection)
                {
                    PrinterSettings ps = new PrinterSettings();
                    ps.PrinterName = printer.Properties["Name"].Value.ToString();

                    if (ps.IsValid)
                        printers.Add(new Printer(ps.PrinterName, ps.PrinterName, ps.IsDefaultPrinter, ps.SupportsColor));
                }

                return printers;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        #endregion

        /// GetPrintJobs
        #region GetPrintJobs
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static StringCollection GetPrintJobNames(string printerName)
        {
            try
            {
                StringCollection printJobCollection = new StringCollection();
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(_searchQueryPrintJobs);
                ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    string prnterName;
                    int prntJobID;
                    ExtractNameAndId(prntJob, out prnterName, out prntJobID);
                    string documentName = prntJob.Properties["Document"].Value.ToString();
                    if (String.Compare(prnterName, printerName, true) == 0)
                        printJobCollection.Add(documentName);
                }
                return printJobCollection;
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static List<ManagementObject> GetPrintJobsX(string printerName)
        {
            try
            {
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(_searchQueryPrintJobs);
                List<ManagementObject> col = new List<ManagementObject>();
                foreach (ManagementObject item in searchPrintJobs.Get())
                {
                    if (ExtractPrinterName(item) == printerName)
                        col.Add(item);
                }
                return col;
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/

        /*
        public static List<ManagementObject> GetPrintJobs()
        {
            try
            {
                StringCollection printJobCollection = new StringCollection();
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(_searchQueryPrintJobs);
                List<ManagementObject> col = new List<ManagementObject>();
                foreach (ManagementObject item in searchPrintJobs.Get())
                    col.Add(item);
                return col;
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/

        #endregion

        /// DefaultPrinter
        #region DefaultPrinter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetDefaultPrinter(String name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultPrinter()
        {
            return new PrinterSettings().PrinterName;
        }

        #endregion

        /// PrintJob
        #region PrintJob

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        public static bool PausePrintJob(ManagementBaseObject printJob, string host)
        {
            return _PauseResumePrintJob(new PrintJobTitle(printJob), "Pause");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        public static bool ResumePrintJob(ManagementBaseObject printJob, string host)
        {
            return _PauseResumePrintJob(new PrintJobTitle(printJob), "Resume");
        }

        public static bool ResumePrintJob(PrintJobData printJob, string host)
        {
            return _PauseResumePrintJob(printJob.PrintJobTitle, "Resume");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <param name="printerName"></param>
        /// <param name="jobId"></param>
        public static void ExtractNameAndId(ManagementBaseObject printJob, out string printerName, out int jobId)
        {
            printerName = null;
            jobId = -1;

            if (printJob == null)
                return;

            try
            {
                //Job name would be of the format [Printer name], [Job ID]
                String jobName = printJob.Properties["Caption"].Value.ToString();
                char[] splitArr = new char[1];
                splitArr[0] = ',';
                printerName = jobName.Split(splitArr)[0];
                jobId = Convert.ToInt32(jobName.Split(splitArr)[1]);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        public static string ExtractPrinterName(ManagementBaseObject printJob)
        {
            if (printJob == null)
                return null;

            try
            {
                String jobName = printJob.Properties["Caption"].Value.ToString();
                char[] splitArr = new char[1];
                splitArr[0] = Convert.ToChar(",");
                return jobName.Split(splitArr)[0];
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        public static string ExtractDocumentName(ManagementBaseObject printJob)
        {
            if (printJob == null)
                return null;

            try
            {
                return printJob.Properties["Document"].Value.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        public static string ExtractHost(ManagementBaseObject printJob)
        {
            if (printJob == null)
                return null;

            try
            {
                return printJob.Properties["HostPrintQueue"].Value.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        public static string ExtractOwner(ManagementBaseObject printJob)
        {
            if (printJob == null)
                return null;

            try
            {
                return printJob.Properties["Owner"].Value.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        /// <returns></returns>
        public static int ExtractJobID(ManagementBaseObject printJob)
        {
            if (printJob == null)
                return -1;

            try
            {
                String jobName = printJob.Properties["Caption"].Value.ToString();
                char[] splitArr = new char[1];
                splitArr[0] = Convert.ToChar(",");
                return Convert.ToInt32(jobName.Split(splitArr)[1]);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return -1;
            }
        }

        private static bool _PauseResumePrintJob(PrintJobTitle title, string methodName)
        {
            if (title.Host == Environment.MachineName)
            {
                return _PauseResumePrintJobLocal(title, methodName);
            }

            try
            {
                LogHelper.LogDebug("Need to " + methodName + " at " + title.Host);
                bool isActionPerformed = false;
                ManagementScope scope = new ManagementScope("\\\\" + title.Host + "\\root\\cimv2");
                scope.Connect();
                ObjectQuery query = new ObjectQuery(_searchQueryPrintJobs);
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
                LogHelper.LogDebug("Enumerate Jobs at " + title.Host + ", " + prntJobCollection.Count);
                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    PrintJobTitle _titleX = new PrintJobTitle(prntJob);
                    if (_titleX.Equals(title))
                    {
                        LogHelper.LogDebug(methodName.Substring(0, methodName.Length - 1) + "ing... " + title.Host);
                        prntJob.InvokeMethod(methodName, null);
                        isActionPerformed = true;
                        LogHelper.LogDebug(methodName + " " + title.Host);
                        //break;
                    }
                }
                return isActionPerformed;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return false;
            }
        }

        private static bool _PauseResumePrintJobLocal(PrintJobTitle title, string methodName)
        {
            try
            {
                LogHelper.LogDebug("Local Need to " + methodName);
                bool isActionPerformed = false;
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(_searchQueryPrintJobs);
                ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
                LogHelper.LogDebug("Local Enumerate Jobs at " + Environment.MachineName + ", " + prntJobCollection.Count);
                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    PrintJobTitle _titleX = new PrintJobTitle(prntJob);
                    if (_titleX.Equals(title))
                    {
                        LogHelper.LogDebug("Local " + methodName.Substring(0, methodName.Length - 1) + "ing...");
                        prntJob.InvokeMethod(methodName, null);
                        isActionPerformed = true;
                        LogHelper.LogDebug(methodName);
                        break;
                    }
                }
                return isActionPerformed;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return false;
            }
        }

        #endregion

        /// CancelPrintJob
        #region CancelPrintJob

            /// <summary>
            /// 
            /// </summary>
            /// <param name="printJob"></param>
            /// <param name="host"></param>
            /// <returns></returns>
        public static bool CancelAllPrintJobs(ManagementBaseObject printJob, string host)
        {
            if (printJob == null)
                return false;

            return CancelAllPrintJobs(new PrintJobTitle(printJob), host);
        }


        public static bool CancelAllPrintJobs(PrintJobData printJob)
        {
            if (printJob == null)
                return false;

            return CancelAllPrintJobs(printJob.PrintJobTitle, printJob.ServerHost);
        }

        public static bool CancelAllPrintJobs(PrintJobTitle printJob, string host)
        {
            try
            {
                LogHelper.LogDebug("Need to Cancel at " + host);
                bool isActionPerformed = false;
                ManagementScope scope = new ManagementScope("\\\\" + host + "\\root\\cimv2");
                scope.Connect();
                ObjectQuery query = new ObjectQuery(_searchQueryPrintJobs);
                ManagementObjectSearcher searchPrintJobs = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection prntJobCollection = searchPrintJobs.Get();
                int cancelCount = 0;
                foreach (ManagementObject prntJob in prntJobCollection)
                {
                    PrintJobTitle _titleX = new PrintJobTitle(prntJob);
                    LogHelper.LogDebug(host + " has to Cancel " + _titleX + " | " + printJob);
                    if (_titleX.Equals(printJob))
                    {
                        LogHelper.LogDebug("Cancelling at " + host);
                        //performs an action similar to the cancel operation of windows print console
                        prntJob.Delete();
                        isActionPerformed = true;
                        cancelCount++;
                    }
                }
                LogHelper.LogDebug("Cancelled " + cancelCount + " | " + host);
                return isActionPerformed;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return false;
            }
        }

        #endregion

        /// PrinterStatus
        #region PrinterStatus

        /// <summary>
        /// Gets printer status of the specified printer
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <returns>printer status</returns>
        public static PrinterTrouble GetPrinterStatus(string host, string printerName, out string statusReport)
        {
            statusReport = "";
            PrinterTrouble printerTrouble = PrinterTrouble.None;

            try
            {
                PrintServer printServer = new PrintServer(@"\\" + host);
                PrintQueueCollection printQueues = printServer.GetPrintQueues();
                foreach (PrintQueue pq in printQueues)
                {
                    if (pq.Name == printerName)
                    {
                        printerTrouble = SpotPrinterTroubles(ref statusReport, pq);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            return printerTrouble;
        }

        /// <summary>
        /// Check for possible trouble states of a printer using its properties
        /// </summary>
        /// <param name="statusReport"></param>
        /// <param name="printQueue"></param>
        public static PrinterTrouble SpotPrinterTroubles(ref String statusReport, PrintQueue printQueue)
        {
            if (printQueue == null)
                return PrinterTrouble.None;

            PrinterTrouble printerTrouble = PrinterTrouble.None;
            printQueue.Refresh();

            if (!(printQueue.HasToner))
            {
                printerTrouble |= PrinterTrouble.OutOfToner;
                statusReport = statusReport + "Is out of toner. ";
            }

            if (printQueue.IsOutOfPaper)
            {
                printerTrouble |= PrinterTrouble.OutOfPaper;
                statusReport = statusReport + "Is out of paper. ";
            }

            if (printQueue.IsOffline)
            {
                printerTrouble |= PrinterTrouble.Offline;
                statusReport = statusReport + "Offline. ";
            }

            if (printQueue.IsInError)
            {
                printerTrouble |= PrinterTrouble.Error;
                statusReport = statusReport + "Error. ";
            }

            if (printQueue.IsPaperJammed)
            {
                printerTrouble |= PrinterTrouble.PaperJammed;
                statusReport = statusReport + "Paper jammed. ";
            }

            return printerTrouble;
        }

        #endregion

        /// SupportedPrintFormat
        #region SupportedPrintFormat

        /// <summary>
        /// 
        /// </summary>
        public enum SupportedPrintFormat
        {
            Unknown = -1,
            None = 0,
            PDF = 1,
            PS = 2,
        }

        #endregion

        /// GetFileFormat
        #region GetFileFormat

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static SupportedPrintFormat GetFileFormat(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                return SupportedPrintFormat.None;

            try
            {
                if (!File.Exists(fileName))
                    return SupportedPrintFormat.None;
            }
            catch
            {
                return SupportedPrintFormat.None;
            }

            if (PDFHelper.IsPDF(fileName))
                return SupportedPrintFormat.PDF;

            if (PostScriptHelper.IsPS(fileName))
                return SupportedPrintFormat.PS;

            return SupportedPrintFormat.Unknown;
        }

        #endregion

        /// HasColor
        #region HasColor

        /// <summary>
        /// Indicates whether specified file has color or not. Has to be supported file (ps or pdf)
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>true if has color; otherwise false</returns>
        public static bool? HasColor(string fileName)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                return null;

            try
            {
                if (!File.Exists(fileName))
                    return null;

                SupportedPrintFormat format = GetFileFormat(fileName);
                if (format == SupportedPrintFormat.PDF)
                    return GhostScriptHelper.HasColor(fileName);
                else if (format == SupportedPrintFormat.PS)
                    return PostScriptHelper.HasColor(fileName);
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            return null;
        }

        #endregion

        /// Variables
        #region Variables

        /// <summary>
        /// SkipMethod
        /// </summary>
        public static AllowPrintDocument SkipMethod;

        /// <summary>
        /// SkipMethod
        /// </summary>
        public static AddJobWatcher AddWatcher;

        #endregion

        /// Print
        #region Print

        /// <summary>
        /// Prints file with specified printer and settings
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="printer">printer settings</param>
        public static void Print(string fileName, string documentName, PrinterSettings printer, PostScriptMetaData data)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                return;

            if (printer == null)
                return;

            if (data == null)
                return;

            try
            {
                // check the existence of the file
                if (!File.Exists(fileName))
                    return;

                // get file format
                SupportedPrintFormat format = GetFileFormat(fileName);
                string newFileName = fileName;
                // if Postscript file
                if (format == SupportedPrintFormat.PS)
                {
                    newFileName = fileName + ".pdf";
                    // convert to .pdf
                    GhostScriptHelper.ConvertPStoPDF(fileName, newFileName);
                }

                // create corresponding print job title
                PrintJobTitle title = new PrintJobTitle(printer.PrinterName, documentName, Environment.UserName, Environment.MachineName, -1);

                // skip this title
                SkipMethod(title);

                // print document
                if (PDFHelper.Print(printer, newFileName, documentName, Environment.UserName, Environment.MachineName))
                {
                    // add title watcher
                    AddWatcher(title);

                    // log printed data
                    LogPrint(ConfigData.FilePath_PrintLog, documentName, printer, data.NumberOfPages);
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }
        
        #endregion

        /// LogPrint
        #region LogPrint

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printJob"></param>
        public static void LogPrint(string fileName, PrintJobData printJob)
        {
            if (printJob == null)
                return;

            LogPrint(fileName, printJob.PrintJobTitle.PrinterName, printJob.PrintJobTitle.Document, printJob.Color, 1, printJob.Duplex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printerName"></param>
        /// <param name="documentName"></param>
        /// <param name="color"></param>
        /// <param name="numberOfPrints"></param>
        /// <param name="duplex"></param>
        private static void LogPrint(string fileName, string printerName, string documentName, bool color, int numberOfPrints, Duplex duplex)
        {
            if (String.IsNullOrWhiteSpace(fileName))
                return;

            if (String.IsNullOrWhiteSpace(printerName))
                return;

            APIWrapper.LogPrint(Environment.MachineName, Environment.UserName, printerName, color, numberOfPrints, duplex != Duplex.Simplex);

            try
            {
                string path = Path.GetDirectoryName(fileName);
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (!Directory.Exists(path))
                        IO.CreateHiddenDirectory(path, false);
                }

                object[] data = new object[] { Environment.MachineName, Environment.UserName, printerName, documentName, color, duplex, numberOfPrints };
                using (FileStream fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Write))
                {
                    LogHelper.LogDebug(fileName);
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Unicode))
                    {
                        sw.WriteLine(data.ToCSV());
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(fileName + " " + ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="documentName"></param>
        /// <param name="printer"></param>
        /// <param name="numberOfPrints"></param>
        public static void LogPrint(string fileName, string documentName, PrinterSettings printer, int numberOfPrints)
        {
            LogPrint(fileName, printer.PrinterName, documentName, printer.DefaultPageSettings.Color, numberOfPrints, printer.Duplex);
        }

        #endregion

        /// PrintTicket
        #region PrintTickets

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static PrintTicket GetPrintTicketFromPrinter(string path, out PrintJobSettings printJobSettings)
        {
            printJobSettings = null;

            if (String.IsNullOrWhiteSpace(path))
                return null;

            PrintQueue printQueue = null;
            PrintServer printServer = new PrintServer();

            // Retrieving collection of local printer on user machine
            PrintQueueCollection localPrinterCollection = printServer.GetPrintQueues();

            foreach (var printer in localPrinterCollection)
            {
                if (printer.Name == path)
                    printQueue = (PrintQueue)printer;
            }

            if (printQueue == null)
                return null;

            printQueue.Refresh();
            // Get default PrintTicket from printer
            //PrintTicket printTicket = printQueue.DefaultPrintTicket;
            PrintTicket printTicket = printQueue.UserPrintTicket;
            PrintJobSettings settings = printQueue.CurrentJobSettings;
            //System.Printing.PrintJobSettings

            printJobSettings = settings;

            return printTicket;
        }

        public static void CancelPrintJobsByUser(string searchhost, string jobhost, string printerName, string owner)
        {
            try
            {
                foreach (ManagementObject job in GetPrintJobs(searchhost, printerName))
                {
                    PrintJobTitle title = new PrintJobTitle(job);
                    if (title.Host == jobhost && title.Owner == owner)
                        CancelAllPrintJobs(title, searchhost);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PrintQueue GetPrintQueue(string name, string host)
        {
            if (String.IsNullOrWhiteSpace(name))
                return null;

            try
            {
                dynamic printServer;
                PrintQueue printQueue = null;
                if (String.IsNullOrWhiteSpace(host) || host == Environment.MachineName)
                    printServer = new LocalPrintServer();
                else
                    printServer = new PrintServer(host);

                if (printServer == null)
                    return null;

                

                //name = name.Replace(host, "");

                LogHelper.LogDebug("SSSS");
                printServer.Refresh();
                LogHelper.LogDebug("Refresh");
                PrintQueueCollection printerCollection = printServer.GetPrintQueues();
                if (printerCollection == null)
                    return null;
                
                LogHelper.LogDebug("Queues " + printerCollection.Count());
                foreach (PrintQueue printer in printerCollection)
                {
                    printer.Refresh();
                    LogHelper.LogDebug("Name " + printer.Name);
                    if (printer.Name == name)
                    {
                        LogHelper.LogDebug("Has queue");
                        printQueue = printer;
                    }
                }

                LogHelper.LogDebug("Out ");
                return printQueue;
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
                return null;
            }
        }

        #endregion

        /// UpdateJobData
        #region UpdateJobData

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="printerName"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        private static bool UpdateJobData(ref PrintJobData data, string printerName, string host)
        {
            bool updated = false;

            try
            {
                LogHelper.LogDebug("INNER START " + printerName);
                PrintQueue queue = GetPrintQueue(printerName, host);
                if (queue == null)
                    return false;

                queue.Refresh();
                if (queue.NumberOfJobs > 0)
                {
                    bool quit = false;
                    while (!quit)
                    {
                        try
                        {
                            LogHelper.LogDebug("jobs " + queue.GetPrintJobInfoCollection().Count() + " | " + queue.NumberOfJobs);
                            foreach (PrintSystemJobInfo info in queue.GetPrintJobInfoCollection())
                            {
                                info.Refresh();
                                string docName = info.Name;
                                int NumberOfPages = info.NumberOfPages;
                                int xxx = info.NumberOfPagesPrinted;
                                LogHelper.LogDebug("Printing " + info.IsPrinting + " | Paused " + info.IsPaused + " | Spooling " + info.IsSpooling + " | IsDeleting " + info.IsDeleting);
                                LogHelper.LogDebug("pages " + NumberOfPages + " printed " + xxx);
                                if (data.PrintJobTitle.Document == docName && data.PrintJobTitle.TotalPages < xxx)
                                {
                                    data.PrintJobTitle.TotalPages = xxx;
                                    updated = true;
                                }
                            }

                            quit = true;
                        }
                        catch (Exception ex)
                        {
                            queue.Refresh();
                            LogHelper.LogDebug("refresh");
                            WPFNotifier.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            LogHelper.LogDebug("INNER END " + printerName);
            return updated;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateJobData(string host, ref PrintJobData data)
        {
            if (data == null)
                return false;

            if (data.PrintJobTitle == null)
                return false;

            if (String.IsNullOrWhiteSpace(data.PrintJobTitle.PrinterName))
                return false;

            bool updated = false;

            try
            {
                string printerName = data.PrintJobTitle.PrinterName;
                if (!String.IsNullOrWhiteSpace(host))
                {
                    printerName = data.PrintJobTitle.PrinterName.Replace(host, "").Trim();
                    if (printerName[0] == '\\')
                        printerName = printerName.Substring(1);

                    LogHelper.LogDebug("MAIN " + host + " printerName " + printerName);
                }
                else
                    LogHelper.LogDebug("MAIN local printerName " + printerName);

                updated = UpdateJobData(ref data, printerName, host);
                /*
                if (!updated)
                {
                    int pos = data.PrintJobTitle.PrinterName.LastIndexOf("\"");
                    string nnn = null;
                    if (pos > 0)
                        nnn = data.PrintJobTitle.PrinterName.Substring(pos + 1);
                    LogHelper.LogDebug("nnn " + nnn);
                    updated = UpdateJobData(ref data, nnn, host);
                }*/
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            return updated;
        }

        #endregion
    }
}
