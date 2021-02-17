using Cubicles.API;
using Cubicles.API.JsonClasses;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;
using CubiclesPrinterLib.Utility;
using CubiclesPrinterUI.Controller;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace CubiclesPrinterUI.Model
{
    /// <summary>
    /// PrintingControlModel class
    /// </summary>
    public sealed class PrintingControlModel
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Timer </summary>
        System.Windows.Forms.Timer timerAllowedToPrint = new System.Windows.Forms.Timer();

        #endregion

        /// Properties
        #region Properties

        /// <summary> PostScript file name </summary>
        public string PSFileName;

        /// <summary> PDF file name </summary>
        public string PDFFileName;

        /// <summary> Processing status </summary>
        public string Status;

        /// <summary> Chosen Printer </summary>
        public Printer ChosenPrinter;

        /// <summary> Printer settings </summary>
        public PrinterSettings PrinterSettings;

        /// <summary> </summary>
        public PrintJobTitle Title;

        /// <summary> Print with color </summary>
        public bool PrintWithColor;

        /// <summary> Can Print with color </summary>
        public bool CanPrintWithColor;

        /// <summary> Print duplex </summary>
        public Duplex PrintDuplex = Duplex.Simplex;

        /// <summary> Print booklet </summary>
        public string PrintBooklet;

        /// <summary> Do PostScript file has color? </summary>
        public bool? FileHasColor = null;

        /// <summary> Number of prints </summary>
        public int NumberOfPrints
        {
            get
            {
                if (NumberOfCopies < 1)
                    NumberOfCopies = 1;

                if (!string.IsNullOrWhiteSpace(PrintBooklet))
                    return (int)Math.Round((NumberOfPages * NumberOfCopies) / 2.0, MidpointRounding.AwayFromZero);
                
                return NumberOfPages * NumberOfCopies;
            }
        }

        /// <summary> Number of pages </summary>
        public int NumberOfPages;

        /// <summary> Number of copies </summary>
        public short NumberOfCopies = 1;

        /// <summary> PostScript file meta data </summary>
        public PostScriptMetaData PostScriptMetaData;

        /// <summary> Printers </summary>
        public Printers Printers;

        /// <summary> Printers </summary>
        public Printers demoPrinters;

        /// <summary> Selected Printer Index </summary>
        public int SelectedPrinterIndex;

        /// <summary> Is Color Virtual Printer Selected </summary>
        public bool IsColorVirtualPrinterSelected = false;

        /// <summary> Is Controls Enabled flag</summary>
        public bool IsControlsEnabled;

        /// <summary> True if real printer caused an action; otherwise false </summary>
        public bool IsBlockedMode = false;

        /// <summary> </summary>
        public bool IsTestMode = false;

        /// <summary> User PC </summary>
        public string UserPC = Environment.MachineName;

        /// <summary> </summary>
        public int ColorPageCost = EnvironmentDataController.Instance.ColorPageCost;

        /// <summary> </summary>
        public int BlackAndWhitePageCost = EnvironmentDataController.Instance.BlackAndWhitePageCost;

        /// <summary> PrintJobData </summary>
        private PrintJobData PrintJobData;

        /// <summary> Flag </summary>
        public bool IsPrintActionPerformed = false;

        #endregion        

        /// Init
        #region Init

        /// <summary>
        /// Constructor. Virtual Printer Mode.
        /// </summary>
        /// <param name="psFileName">postscript file name</param>
        public PrintingControlModel(string psFileName, PrintJobTitle title)
        {
            IsBlockedMode = false;
            PSFileName = psFileName;
            Title = title;
            if (Title == null)
                Title = new PrintJobTitle(null, "unknown", null, null, -1);

            if (Title.PrinterName.Equals(ConfigData.PrinterName2))
                IsColorVirtualPrinterSelected = true;

            ChosenPrinter = new Printer();
            IsControlsEnabled = false;
            SelectedPrinterIndex = 0;
            Printers = new Printers();

            // initialize timer
            timerAllowedToPrint = new System.Windows.Forms.Timer();
            timerAllowedToPrint.Enabled = false;
            timerAllowedToPrint.Interval = ConfigData.Interval_TimerAllowedToPrint;
            timerAllowedToPrint.Tick += timerAllowedToPrint_Tick;
        }

        /// <summary>
        /// Constructor. Real Printer Mode.
        /// </summary>
        /// <param name="data">print job data</param>
        public PrintingControlModel(PrintJobData data)
        {
            IsBlockedMode = true;
            PSFileName = null;
            PrintJobData = data;
            Title = PrintJobData.PrintJobTitle;
            if (Title == null)
                Title = new PrintJobTitle(null, "unknown", null, null, -1);

            NumberOfPages = Title.TotalPages;

            // set the chosen printer accordingly
            ChosenPrinter = new Printer();
            ChosenPrinter.Name = Title.PrinterName;
            ChosenPrinter.DisplayName = Title.PrinterName;
            IsControlsEnabled = true;
            SelectedPrinterIndex = -1;
            Printers = new Printers();
            //Printers.Add(ChosenPrinter);

            // initialize timer
            timerAllowedToPrint = new System.Windows.Forms.Timer();
            timerAllowedToPrint.Enabled = false;
            timerAllowedToPrint.Interval = ConfigData.Interval_TimerAllowedToResumeJob;
            timerAllowedToPrint.Tick += timerAllowedToPrint_Tick;

            UpdateDataDevMode();
        }

        /// <summary>
        /// Constructor. Demo Mode.
        /// </summary>
        /// <param name="userPC">user PC name</param>
        /// <param name="documentName">document name</param>
        /// <param name="pageCount">page count</param>
        /// <param name="colorPageCost">color page cost</param>
        /// <param name="blackAndWhitePageCost">black and white page cost</param>
        /// <param name="isColorDocument">is color document flag</param>
        /// <param name="printers">list of printers</param>
        public PrintingControlModel(string userPC, string documentName, int pageCount, int colorPageCost, int blackAndWhitePageCost, bool isColorDocument, Printers printers)
        {
            IsTestMode = true;
            NumberOfPages = pageCount;
            PostScriptMetaData = new PostScriptMetaData() { Title = documentName, NumberOfPages = pageCount, HasColor = isColorDocument };
            FileHasColor = isColorDocument;
            UserPC = userPC;
            ColorPageCost = colorPageCost;
            BlackAndWhitePageCost = blackAndWhitePageCost;
            Printers = printers;
            SelectedPrinterIndex = 0;
            ChosenPrinter = Printers[0];
            IsControlsEnabled = true;

            demoPrinters = new Printers();
            foreach(var item in printers)
                demoPrinters.Add(item);
        }

        /// <summary>
        /// Initializes specific data
        /// </summary>
        public void Init()
        {
            // check if it's a Demo mode
            if (IsTestMode)
            {
                SetStatus("Ready to Print");
                OnReadyToPrint(EventArgs.Empty);
                return;
            }

            LogHelper.LogDebug();

            // check if it's real printer mode
            if (IsBlockedMode)
            {
                SetStatus("Ready to Print");
                OnReadyToPrint(EventArgs.Empty);
            }
            else // virtual printer mode
            {
                // set status
                SetStatus("Loading");

                GetDataAsync();
            }
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Gets the data asynchronously
        /// </summary>
        private void GetDataAsync()
        {
            CancellationTokenSource source = new CancellationTokenSource();

            // main auxiliary task to perform subtasks and notify system when everything is done
            Task t = new Task(() =>
            {
                // task to update printers list
                Task t1 = new Task(() =>
                {
                    SetStatus("Update Printers");
                    // update printers
                    if (!UpdatePrinters())
                        source.Cancel(false);
                    SetStatus();
                });

                // task to convert PS to PDF and determine colors
                Task t2 = new Task(() =>
                {
                    // get colors
                    // set status
                    PDFFileName = PSFileName + ".pdf";
                    SetStatus("Convert to PDF");
                    GhostScriptHelper.ConvertPStoPDF(PSFileName, PDFFileName);
                    SetStatus("Determine colors");
                    FileHasColor = PDFHelper.HasColor(PDFFileName);
                    OnColorDetermined(EventArgs.Empty);
                    SetStatus();
                });

                // task to get metadata from PS file
                Task t3 = new Task(() =>
                {
                    // get postscript data
                    SetStatus("Get Print Data");
                    GetPSData(PSFileName);
                    OnColorDetermined(EventArgs.Empty);
                    SetStatus();
                });

                t1.Start();
                t2.Start();
                t3.Start();

                // wait until every auxiliary task is completed
                Task.WaitAll(new Task[] {t1, t2, t3}, source.Token);
                LogHelper.LogDebug("Task.WaitAll");

                if (PostScriptMetaData != null)
                    PostScriptMetaData.HasColor = FileHasColor;

                SetStatus("Set Printers");
                // change printers
                ChangePrinters();

                // release controls
                IsControlsEnabled = true;
                LogHelper.LogDebug("ReadyToPrint Before");

                SetPrinterByVirtualPrinterSelected();

                SetStatus("Ready to Print");
                // notify system about readiness
                OnReadyToPrint(EventArgs.Empty);
            });

            t.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetPrinterByVirtualPrinterSelected()
        {
            try
            {
                // check printers
                if (Printers != null && Printers.Count > 0)
                {
                    // select color printers
                    var cps = Printers.Where(x => x.IsColored == IsColorVirtualPrinterSelected);
                    // check color printers
                    if (cps != null && cps.Count() > 0)
                    {
                        Printer cp = null;
                        // select default color printer if available
                        var cpsd = cps.Where(x => x.IsDefault);
                        if (cpsd != null && cpsd.Count() > 0)
                            cp = cpsd.First();
                        // check if color printer exists
                        if (cp == null)
                            cp = cps.First();
                        // check if color printer exists
                        if (cp != null)
                            ChosenPrinter = cp;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogDebug(ex);
            }
        }

        /// <summary>
        /// Closes window.
        /// </summary>
        public void Close()
        {
            IsPrintActionPerformed = true;

            OnClosed(EventArgs.Empty);
        }

        /// <summary>
        /// Shows advanced settings window.
        /// </summary>
        /// <param name="owner">owner</param>
        public void ShowAdvancedSettings(Window owner)
        {
            if (IsBlockedMode)
                return;

            try
            {
                // get chosen printer by color setting
                Printer pr = GetChosenPrinterWithSettings(PrintWithColor);
                if (pr == null)
                    return;

                // initialize settings extractor
                UISettingsExtractor extractor = new UISettingsExtractor(ChosenPrinter.Name, DesktopHelper.GetDesktopWindow());

                // start extractor
                extractor.Start();

                // show advanced printer settings
                MainController.Singleton.ShowAdvancedPrinterSettings(new WindowInteropHelper(owner).Handle, ChosenPrinter.Name, ref PrinterSettings);

                // stop extractor
                extractor.Stop();

                // get booklet data from extractor
                PrintBooklet = extractor.Booklet;
                LogHelper.LogDebug(PrintBooklet);

                // set changed printer settings
                pr.Settings = PrinterSettings;

                // set UI data by printer settings
                SetByPrinterSettings();
            }
            catch(Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }        

        /// <summary>
        /// Gets chosen printer and its' settings
        /// </summary>
        /// <returns>chosen printer</returns>
        public Printer GetChosenPrinterWithSettings(bool colored)
        {
            if (IsBlockedMode)
                return ChosenPrinter;

            // check the selected index
            if (SelectedPrinterIndex < 0)
                return null;            

            try
            {
                LogHelper.LogDebug();

                // get chosen printer
                Printer PrinterX = null;
                if (IsTestMode)                    
                    PrinterX = demoPrinters.ByColor(colored)[SelectedPrinterIndex];
                else
                    PrinterX = MainController.Singleton.AvailablePrinters.ByColor(colored)[SelectedPrinterIndex];

                // set chosen printer
                ChosenPrinter = PrinterX;

                // check settings
                if (PrinterX.Settings == null)
                {
                    // first time settings set

                    // fullfill some important settings values
                    PrinterSettings = new PrinterSettings() { PrinterName = ChosenPrinter.Name, Duplex = PrintDuplex, DefaultPageSettings = { Color = colored } };

                    // validate settings
                    if (!PrinterSettings.IsValid)
                        if (!IsTestMode)
                            throw new NotSupportedException("Wrong printer settings");

                    // set settings
                    PrinterX.Settings = PrinterSettings;
                }
                else
                {
                    // set settings by saved settings set
                    PrinterSettings = PrinterX.Settings;
                    PrinterSettings.Duplex = PrintDuplex;
                    PrinterSettings.DefaultPageSettings.Color = colored;
                    PrinterSettings.Copies = NumberOfCopies;
                }

                return PrinterX;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Update data by DEVMODE structure.
        /// </summary>
        private void UpdateDataDevMode()
        {
            try
            {
                // get job info
                var jobInfo2 = CubiclesPrinterLib.Win32.Print.PrintJob.GetJobInfo(PrintJobData.RemotePrinterName, PrintJobData.PrintJobTitle.JobID);
                if (jobInfo2 != null)
                {
                    NumberOfCopies = (short)jobInfo2.dmOut.dmCopies;
                    PrintDuplex = (Duplex)jobInfo2.dmOut.dmDuplex;
                }
                else
                    WPFNotifier.Warning(PrintJobData.RemotePrinterName + Environment.NewLine + " Can't access printer, data corrupted, default values set.");
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }
        }

        /// <summary>
        /// Updates data.
        /// </summary>
        /// <param name="data">updated data to be used</param>
        public void UpdateData(PrintJobData data)
        {
            // check data
            if (data == null)
            {
                LogHelper.LogDebug("Update data but nothing received");
                return;
            }

            try
            {
                LogHelper.LogDebug("Update data, pages " + data.PrintJobTitle.TotalPages);

                // update data if more pages found
                if (data.PrintJobTitle.TotalPages > PrintJobData.PrintJobTitle.TotalPages)
                {
                    LogHelper.LogDebug("Current pages " + PrintJobData.PrintJobTitle.TotalPages);

                    // set data
                    PrintJobData = data;

                    // update values
                    Title = data.PrintJobTitle;
                    NumberOfPages = data.PrintJobTitle.TotalPages;

                    // set status
                    SetStatus("Ready to print");
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
            }

            UpdateDataDevMode();
        }

        /// <summary>
        /// Resets demo printers.
        /// </summary>
        private void ResetDemoPrinters()
        {
            if (IsBlockedMode)
                return;

            Printers = new Printers();
            foreach(var item in demoPrinters)
                Printers.Add(item);
        }

        /// <summary>
        /// Selects recommended printer.
        /// </summary>
        /// <param name="PrintWithColor">printer color flag</param>
        public void SelectRecommendedPrinter(bool PrintWithColor)
        {
            // check if real printer selected
            if (IsBlockedMode)
                return;

            // select recommended printer
            ChosenPrinter = Printers.Where(printer => printer.IsColored == PrintWithColor && printer.IsDefault).FirstOrDefault();
            // check the recommended printer
            if (ChosenPrinter == null)
                // reselect
                ChosenPrinter = Printers.Where(printer => printer.IsColored == PrintWithColor).FirstOrDefault();
        }

        /// <summary>
        /// Sets special variables by printer settings.
        /// </summary>
        private void SetByPrinterSettings()
        {
            try
            {
                // check the printer settings
                if (PrinterSettings != null)
                {
                    PrintDuplex = PrinterSettings.Duplex;
                    PrintWithColor = PrinterSettings.DefaultPageSettings.Color;
                    //NumberOfPages = PostScriptMetaData.NumberOfPages;
                    NumberOfCopies = PrinterSettings.Copies;

                    if (!string.IsNullOrWhiteSpace(PrintBooklet))
                    {
                        if (PrintBooklet.Contains("2-sided"))
                            PrintBooklet = null;
                        else
                        if (PrintBooklet.Equals("None"))
                            PrintBooklet = null;
                    }

                    LogHelper.LogDebug(PrintBooklet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            SetStatus();
        }

        /// <summary>
        /// Gets Postscript Data.
        /// </summary>
        /// <param name="psFileName">postscript file</param>
        private void GetPSData(string psFileName)
        {
            LogHelper.LogDebug();

            // set status
            SetStatus("Get metadata");
            PostScriptMetaData = PostScriptHelper.GetData(psFileName);
            // check postscript data
            if (PostScriptMetaData != null)
            {
                PostScriptMetaData.HasColor = FileHasColor;

                if (Title != null)
                    PostScriptMetaData.Title = Title.Document;

                NumberOfPages = PostScriptMetaData.NumberOfPages;
            }
            else
            {
                NumberOfPages = -1;
            }
        }

        /// <summary>
        /// Sets default printer.
        /// </summary>
        private void SetDefaultPrinter()
        {
            LogHelper.LogDebug();

            // check if it's Demo mode
            if (IsTestMode)
            {
                Printer pr = demoPrinters.DefaultPrinter;

                if (!demoPrinters.Contains(pr))
                    SelectedPrinterIndex = 0;
                else
                    SelectedPrinterIndex = demoPrinters.IndexOf(pr);

                ChosenPrinter = demoPrinters.ByColor(PrintWithColor)[SelectedPrinterIndex];
            }
            else
            {
                if (MainController.Singleton.HasAvailablePrinters)
                {
                    Printer pr = MainController.Singleton.AvailablePrinters.DefaultPrinter;
                    if (pr != null)
                    {
                        if (!Printers.Contains(pr))
                            SelectedPrinterIndex = 0;
                        else
                            SelectedPrinterIndex = Printers.IndexOf(pr);

                        ChosenPrinter = MainController.Singleton.AvailablePrinters.ByColor(PrintWithColor)[SelectedPrinterIndex];
                    }
                    else
                    {
                        ChosenPrinter = MainController.Singleton.AvailablePrinters[0];
                    }
                }
            }
        }

        /// <summary>
        /// Loads list of printers
        /// </summary>
        private bool UpdatePrinters()
        {
            LogHelper.LogDebug();

            // check if it's Demo mode
            if (IsTestMode)
            {
                ResetDemoPrinters();
                return true;
            }

            MainController.Singleton.UpdateAvailablePrinters();

            if (!MainController.Singleton.HasAvailablePrinters)
            {
                WPFNotifier.Warning("This printer is not available now. Try to use one of the other printers.");
                Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Changes list of printers
        /// </summary>
        public void ChangePrinters()
        {
            LogHelper.LogDebug();

            // check if it's Demo mode
            if (IsTestMode)
            {
                ResetDemoPrinters();

                bool nocolored = false;
                CanPrintWithColor = true;
                Printers printers = null;
                if (PrintWithColor)
                {
                    printers = Printers.Colored;
                    if (printers == null || printers.Count < 1)
                        nocolored = true;
                }
                else
                    printers = demoPrinters;

                if (nocolored)
                {
                    LogHelper.LogDebug("No available color printers found. You are not allowed to print colored this time.");
                    WPFNotifier.Warning("No available color printers found. You are not allowed to print colored this time.");
                    CanPrintWithColor = false;
                    return;
                }

                if (printers == null || printers.Count < 1)
                {
                    LogHelper.LogDebug("No available printers found. You are not allowed to print this time.");
                    WPFNotifier.Warning("No available printers found. You are not allowed to print this time.");
                    return;
                }

                LogHelper.LogDebug("Add printers to combo : " + printers.Count);

                Printers.Clear();
                foreach (var name in printers.ToArray())
                {
                    LogHelper.LogDebug("Adding  : " + name);
                    Printers.Add(name);
                }

                LogHelper.LogDebug("Added printers to combo : " + Printers.Count);

                Thread.Sleep(100);
                SetDefaultPrinter();
            }
            else
            {
                try
                {
                    bool nocolored = false;
                    CanPrintWithColor = true;
                    Printers printers = null;
                    if (PrintWithColor)
                    {
                        printers = MainController.Singleton.AvailablePrinters.Colored;
                        if (printers == null || printers.Count < 1)
                            nocolored = true;
                    }
                    else
                        printers = MainController.Singleton.AvailablePrinters;

                    if (nocolored)
                    {
                        LogHelper.LogDebug("No available color printers found. You are not allowed to print colored this time.");
                        WPFNotifier.Warning("No available color printers found. You are not allowed to print colored this time.");
                        CanPrintWithColor = false;
                        return;
                    }

                    if (printers == null || printers.Count < 1)
                    {
                        LogHelper.LogDebug("No available printers found. You are not allowed to print this time.");
                        WPFNotifier.Warning("No available printers found. You are not allowed to print this time.");
                        return;
                    }

                    LogHelper.LogDebug("Add printers to combo : " + printers.Count);

                    MainController.Singleton.Dispatcher.Invoke(new Action(() =>
                    {
                        try
                        {
                            Printers.Clear();
                            foreach (var name in printers.ToArray())
                            {
                                LogHelper.LogDebug("Adding  : " + name);
                                Printers.Add(name);
                            }

                            LogHelper.LogDebug("Added printers to combo : " + Printers.Count);
                        }
                        catch(Exception ex)
                        {
                            WPFNotifier.Error("XXX.", null, ex);
                        }
                        
                    }), DispatcherPriority.Background);

                    Thread.Sleep(100);
                    SetDefaultPrinter();
                }
                catch (Exception ex)
                {
                    WPFNotifier.Error("This printer is not available now. Try to use one of the other printers.", null, ex);
                    Close();
                }
            }
        }

        /// <summary>
        /// Sets status of the task
        /// </summary>
        /// <param name="stat"></param>
        private void SetStatus(string stat = null)
        {
            if (!string.IsNullOrWhiteSpace(stat))
                Status = stat;

            OnStatusChanged(EventArgs.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printJob"></param>
        private void ResumeJob(PrintJobData printJob)
        {
            // check if it's Demo mode
            if (IsTestMode)
                return;

            SetStatus("Printing");
            MainController.Singleton.AddWatcher(printJob);
            MainController.Singleton.SkipDocument(printJob.PrintJobTitle);
            PrintHelper.ResumePrintJob(printJob, printJob.ServerHost);
            PrintHelper.LogPrint(ConfigData.FilePath_PrintLog, printJob);
            PrintJobData = null;
            Close();
        }

        /// <summary>
        /// Cancels print job
        /// </summary>
        /// <param name="printJob"></param>
        private void CancelJob(PrintJobData printJob)
        {
            if (printJob == null)
                return;

            SetStatus("Cancelling print job");
            //PrintHelper.CancelPrintJob(printJob);
            PrintHelper.CancelAllPrintJobs(printJob);
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Cancel()
        {
            CancelJob(PrintJobData);
        }

        #endregion

        /// Print
        #region Print

        /// <summary>
        /// Prints data.
        /// </summary>
        public void Print()
        {
            // check if it's Demo mode
            if (IsTestMode)
                return;

            IsControlsEnabled = false;
            SetStatus();

            Task t = new Task(() =>
            {
                try
                {
                    if (IsBlockedMode)
                    {
                        ResumeJobFirstTry(PrintJobData);
                    }
                    else
                    {
                        PrintFirstTry(PDFFileName, Title.Document, ChosenPrinter.Settings);
                    }
                }
                catch(Exception ex)
                {
                    WPFNotifier.Error(ex);
                }
                
            });

            t.Start();
        }

        /// <summary>
        /// Tries to resume the print job first time
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printJob"></param>
        void ResumeJobFirstTry(PrintJobData printJob)
        {
            if (ConfigData.Config_DontCheckUserBalance)
            {
                ResumeJob(printJob);
            }
            else
            {
                SetStatus("Checking user balance");
                var res = APIWrapper.CheckIfCanPrint(Environment.MachineName, Environment.UserName, printJob.Color, NumberOfPrints, printJob.PrintJobTitle.PrinterName);
                if (res == null)
                {
                    WPFNotifier.Warning(string.Format("No response from server but still printing."));
                    ResumeJob(printJob);
                }
                else
                {
                    if (res.Result)
                    {
                        if (res.AllowedToPrint)
                        {
                            ResumeJob(printJob);
                        }
                        else
                        {
                            SetStatus("Not allowed to print");
                            if (WPFNotifier.Question(string.Format("You are not allowed to print at the current time.{0}Reason: {1}{0}{2}", Environment.NewLine, res.Reason, "Do you want to leave the printing (Yes) or continue waiting (No)?")) == MessageBoxResult.Yes)
                                CancelJob(printJob);
                            else
                                StartTimerAllowedToPrint();
                        }
                    }
                    else
                    {
                        WPFNotifier.Warning(string.Format("Not successful request CheckIfCanPrint."));
                    }
                }
            }
        }

        /// <summary>
        /// Tries to print first time 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printerName"></param>
        /// <param name="printerSettings"></param>
        void PrintFirstTry(string fileName, string documentName, PrinterSettings printerSettings)
        {
            // check if it's Demo mode
            if (IsTestMode)
                return;

            if (ConfigData.Config_DontCheckUserBalance)
            {
                Print(fileName, documentName, printerSettings, PostScriptMetaData);
            }
            else
            {
                SetStatus("Checking user balance");
                var res = APIWrapper.CheckIfCanPrint(Environment.MachineName, Environment.UserName, printerSettings.DefaultPageSettings.Color, NumberOfPrints, ChosenPrinter.Name);
                if (res == null)
                {
                    WPFNotifier.Warning(string.Format("No response from server but still printing."));
                    Print(fileName, documentName, printerSettings, PostScriptMetaData);
                }
                else
                {
                    if (res.Result)
                    {
                        if (res.AllowedToPrint)
                        {
                            Print(fileName, documentName, printerSettings, PostScriptMetaData);
                        }
                        else
                        {
                            //WPFNotifier.Warning(string.Format("You are not allowed to print in the current time.{0}Reason: {1}", Environment.NewLine, res.Reason));
                            SetStatus("Not allowed to print");
                            if (WPFNotifier.Question(string.Format("You are not allowed to print at the current time.{0}Reason: {1}{0}{2}", Environment.NewLine, res.Reason, "Do you want to leave the printing (Yes) or continue waiting (No)?")) == MessageBoxResult.Yes)
                                Close();
                            else
                                StartTimerAllowedToPrint();
                        }
                    }
                    else
                    {
                        WPFNotifier.Warning(string.Format("Not successful request CheckIfCanPrint."));
                    }
                }
            }
        }

        /// <summary>
        /// Prints file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printerName"></param>
        /// <param name="printerSettings"></param>
        private void Print(string fileName, string documentName, PrinterSettings printerSettings, PostScriptMetaData data)
        {
            // check if it's Demo mode
            if (IsTestMode)
                return;

            SetStatus("Printing");
            LogHelper.Log("Printing " + fileName + " (" + documentName + ") with " + printerSettings.PrinterName);
            PrintHelper.Print(fileName, documentName, printerSettings, data);
            LogHelper.LogDebug("Printing Completed");
            Close();
        }

        #endregion

        /// timerAllowedToPrint
        #region timerAllowedToPrint

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerAllowedToPrint_Tick(object sender, EventArgs e)
        {
            timerAllowedToPrint.Stop();

            if (!ConfigData.Config_DontCheckUserBalance)
            {
                AllowedToPrintResponse res = APIWrapper.CheckIfCanPrint(Environment.MachineName, Environment.UserName, PrintWithColor, NumberOfPrints, ChosenPrinter.Name);
                if (res == null)
                {
                    WPFNotifier.Warning(string.Format("No response from server but still printing."));
                    Print(PDFFileName, PostScriptMetaData.Title, ChosenPrinter.Settings, PostScriptMetaData);
                }
                else
                {
                    if (res.AllowedToPrint)
                    {
                        Print(PDFFileName, PostScriptMetaData.Title, ChosenPrinter.Settings, PostScriptMetaData);
                    }
                    else
                    {
                        timerAllowedToPrint.Start();
                    }
                }
            }

            timerAllowedToPrint.Start();
        }        

        /// <summary>
        /// Starts timer.
        /// </summary>
        private void StartTimerAllowedToPrint()
        {
            IsControlsEnabled = false;
            timerAllowedToPrint.Start();
        }

        /// <summary>
        /// Stops timer.
        /// </summary>
        private void StopTimerAllowedToPrint()
        {
            timerAllowedToPrint.Stop();
            IsControlsEnabled = true;
        }   

        #endregion

        /// Events
        #region Events

        /// <summary>
        /// Occurs when the window has to be closed
        /// </summary>
        public event EventHandler<EventArgs> Closed;

        /// <summary>
        /// Raises 'Closed' event.
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnClosed(EventArgs e)
        {
            EventHandler<EventArgs> handler = Closed;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs when the status has been changed
        /// </summary>
        public event EventHandler<EventArgs> StatusChanged;

        /// <summary>
        /// Raises 'StatusChanged' event.
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnStatusChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = StatusChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs when the color of the job has been determined
        /// </summary>
        public event EventHandler<EventArgs> ColorDetermined;

        /// <summary>
        /// Raises 'ColorDetermined' event.
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnColorDetermined(EventArgs e)
        {
            EventHandler<EventArgs> handler = ColorDetermined;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Occurs when the app is ready to print
        /// </summary>
        public event EventHandler<EventArgs> ReadyToPrint;

        /// <summary>
        /// Raises 'ReadyToPrint' event.
        /// </summary>
        /// <param name="e">EventArgs</param>
        private void OnReadyToPrint(EventArgs e)
        {
            EventHandler<EventArgs> handler = ReadyToPrint;
            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
