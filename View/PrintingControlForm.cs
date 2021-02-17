using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Cubicles.API;
using Cubicles.API.JsonClasses;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinter.Controller;
using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;
using CubiclesPrinterLib.Utility;

namespace CubiclesPrinter.View
{
    /// <summary>
    /// This class represenys Printing Control Form 
    /// </summary>
    public partial class PrintingControlForm : Form
    {
        /// Private Variables
        #region Private Variables

        /// <summary> PostScript file name </summary>
        private string _psFileName;

        /// <summary> Processing status </summary>
        private string _status;

        /// <summary> Printer name </summary>
        private string _printerName;

        /// <summary> Printer job </summary>
        //private PrintJobData _printJob;

        /// <summary> Printer settings </summary>
        private PrinterSettings _printerSettings;

        /// <summary> </summary>
        private PrintJobTitle _title;

        /// <summary> Print with color </summary>
        private bool _printWithColor;

        /// <summary> Print duplex </summary>
        private Duplex _printDuplex;

        /// <summary> Print booklet </summary>
        private string _printBooklet;

        /// <summary> Do PostScript file has color? </summary>
        private bool? _psFileHasColor = null;

        /// <summary> Number of prints </summary>
        private int _numberOfPrints;

        /// <summary> Number of copies </summary>
        private short _numberOfCopies;

        /// <summary> PostScript file meta data </summary>
        private PostScriptMetaData _psFileData;

        /// <summary> Timer </summary>
        System.Windows.Forms.Timer timerAllowedToPrint = new System.Windows.Forms.Timer();   

        #endregion

        /// Properties
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        //public PrintJobData PrintJobData { get { return _printJob; }}

        /// <summary>
        /// 
        /// </summary>
        public PrintJobTitle PrintJobTitle { get { return _title; } }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName"></param>
        public PrintingControlForm(string fileName, PrintJobTitle title)
        {
            _psFileName = fileName;
            _title = title;

            InitializeComponent();

            Init();
        }

        /// <summary>
        /// Initializes specific data
        /// </summary>
        private void Init()
        {
            LogHelper.LogDebug();

            MainController.Singleton.SetDefaultPrinter();

            // set status
            SetStatus("Init");
            
            // initialize timer
            timerAllowedToPrint = new System.Windows.Forms.Timer();
            timerAllowedToPrint.Enabled = false;
            timerAllowedToPrint.Interval = ConfigData.Interval_TimerAllowedToPrint;
            timerAllowedToPrint.Tick += timerAllowedToPrint_Tick;

            comboBoxDuplex.SelectedIndex = 0;

            panelControl.Enabled = false;

            // set status
            SetStatus("Loading");
        }

        void DataReadingCompleted(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                // set status
                SetStatus("Printer setup");

                // set UI by color
                SetUIByColor();

                // set top most
                SetTopMost();

                // set status
                SetStatus("Ready to print");

                // enable interactions
                panelControl.Enabled = true;
            });
        }

        void DataReadingStarted(object sender, EventArgs e)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                // set status
                SetStatus("Initialization");
                panelControl.Enabled = false;
            });
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        private void ShowForm()
        {
            SetTopMost();

            // set status
            SetStatus("Ready to print");

            SetTopMost();

            panelControl.Enabled = true;
        }

        private void GetDataAsync(string fileName)
        {
            Task t = new Task(() =>
            {
                DataReadingStarted(this, EventArgs.Empty);
                _GetData(fileName);
                DataReadingCompleted(this, null);
                
            });

            t.Start();
        }

        private void _GetData(string fileName)
        {
            // load printers data
            if (!UpdatePrinters())
                return;

            // get postscript data
            GetPSData(fileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        private void _SetStatus(string stat = null)
        {
            if (!string.IsNullOrWhiteSpace(stat))
                _status = stat;

            labelStatusTitle.Text = string.Format("Status: {0} | Printer = {1}",
                _status,
                string.IsNullOrWhiteSpace(comboBoxPrinters.Text) ? "Not chosen" : comboBoxPrinters.Text);

            labelStatus.Text = string.Format("Document = {1}{0}Number of prints = {2} | Has Color = {3} | Print Colored = {4} && Double-Sided = {5}{6}",
                Environment.NewLine,
                _psFileData == null ? "Unknown" : (string.IsNullOrWhiteSpace(_psFileData.Title) ? "Unknown" : _psFileData.Title.Short()),
                _numberOfPrints < 1 ? "Unknown" : (_numberOfPrints*(_printerSettings == null ? (_numberOfCopies > 0 ? _numberOfCopies : 1) : _printerSettings.Copies < 1 ? 1 : _printerSettings.Copies)).ToString(),
                _psFileHasColor.HasValue ? (_psFileHasColor.Value ? "Yes" : "No") : "Unknown",
                radioButtonColored.Checked ? "Yes" : "No",
                (_printDuplex == Duplex.Simplex || _printDuplex == Duplex.Default) ? "No" : "Yes",
                string.IsNullOrWhiteSpace(_printBooklet) ? "" : string.Format(" ( {0} )", _printBooklet));
                //comboBoxDuplex.SelectedIndex != 0 ? "Yes" : "No");

            //LogHelper.LogDebug("PrintDuplex " + _printDuplex);
            labelStatusTitle.Invalidate();

            Text = string.Format("{0} - {1}", ConfigData.AppName, _status);

            Invalidate();
            Refresh();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stat"></param>
        private void SetStatus(string stat = null)
        {
            if (InvokeRequired)
                Invoke((MethodInvoker) delegate { _SetStatus(stat); });
            else
                _SetStatus(stat);
        }

        /// <summary>
        /// Sets default printer
        /// </summary>
        private void SetDefaultPrinter()
        {
            LogHelper.LogDebug();
            if (MainController.Singleton.HasAvailablePrinters)
            {
                LogHelper.LogDebug("Has printers " + MainController.Singleton.AvailablePrinters.DefaultPrinterName);
                comboBoxPrinters.Text = MainController.Singleton.AvailablePrinters.DefaultPrinterName;
                if (string.IsNullOrWhiteSpace(comboBoxPrinters.Text))
                    comboBoxPrinters.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Loads list of printers
        /// </summary>
        private bool UpdatePrinters()
        {
            LogHelper.LogDebug();

            MainController.Singleton.UpdateAvailablePrinters();

            if (!MainController.Singleton.HasAvailablePrinters)
            {
                Notifier.Warning("This printer is not available now. Try to use one of the other printers.");
                Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Changes list of printers
        /// </summary>
        private void ChangePrinters()
        {
            LogHelper.LogDebug();
            comboBoxPrinters.Items.Clear();

            try
            {
                bool nocolored = false;
                List<string> printers = null;
                if (radioButtonColored.Checked)
                {
                    printers = MainController.Singleton.AvailablePrinters.ColoredNames;
                    if (printers == null || printers.Count < 1)
                        nocolored = true;
                }
                else
                    printers = MainController.Singleton.AvailablePrinters.AllNames;

                if (nocolored)
                {
                    LogHelper.LogDebug("No available color printers found. You are not allowed to print colored this time.");
                    Notifier.Warning("No available color printers found. You are not allowed to print colored this time.");
                    radioButtonColored.Enabled = false;
                    radioButtonGrayScale.Checked = true;
                    return;
                }

                if (printers == null || printers.Count < 1)
                {
                    LogHelper.LogDebug("No available printers found. You are not allowed to print this time.");
                    Notifier.Warning("No available printers found. You are not allowed to print this time.");
                    return;
                }

                LogHelper.LogDebug("Add printers to combo : " + printers.Count);
                comboBoxPrinters.Items.AddRange(printers.ToArray());
                Thread.Sleep(100);
                SetDefaultPrinter();
            }
            catch (Exception ex)
            {
                Notifier.Error("This printer is not available now. Try to use one of the other printers.", null, ex);
                Close();
            }
        }

        /// <summary>
        /// Gets Postscript Data
        /// </summary>
        /// <param name="psFileName">postscript file</param>
        private void GetPSData(string psFileName)
        {
            LogHelper.LogDebug();

            // set status
            SetStatus("Determine colors");
            _psFileHasColor = PostScriptHelper.HasColor(psFileName);

            // set status
            SetStatus("Get metadata");
            _psFileData = PostScriptHelper.GetData(psFileName);
            _psFileData.HasColor = _psFileHasColor;

            if (_title != null)
                _psFileData.Title = _title.Document;

            if (_psFileData == null)
                _numberOfPrints = -1;
            else
                _numberOfPrints = _psFileData.NumberOfPages;
        }
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        public void CheckPrinterIssuesAndSend(string host, string printerName)
        {
            SetStatus("Checking printer status.");
            string status = "";
            PrinterTrouble pt = PrintHelper.GetPrinterStatus(host, printerName, out status);
            if (pt != PrinterTrouble.None)
            {
                SetStatus("Printer issue found. Sending.");
                APIWrapper.PrinterIssue(printerName, pt);
                return;
            }
            SetStatus("Printer available.");
        }

        
        
        /// <summary>
        /// Sets UI by color option
        /// </summary>
        private void SetUIByColor()
        {
            if (!_psFileHasColor.HasValue)
            {
                radioButtonColored.Enabled = false;
                radioButtonGrayScale.Checked = true;
            }
            else
            {
                if (_psFileHasColor.Value)
                {
                    radioButtonColored.Enabled = true;
                    radioButtonColored.Checked = true;
                }
                else
                {
                    radioButtonColored.Enabled = false;
                    radioButtonGrayScale.Checked = true;
                }
            }
        }

        /// <summary>
        /// Prints file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printerName"></param>
        /// <param name="printerSettings"></param>
        private void Print(string fileName, string printerName, string documentName, PrinterSettings printerSettings, PostScriptMetaData data)
        {
            //PrintJobData printJob = new PrintJobData(){PrintJobTitle = new PrintJobTitle(printerName)}

            SetStatus("Printing");
            ShowInTaskbar = false;
            Hide();
            //MainController.Singleton.AddWatcher(printJob);
            //MainController.Singleton.SkipDocument(printJob.PrintJobTitle.PrinterName, printJob.PrintJobTitle.Document, printJob.PrintJobTitle.Owner, printJob.PrintJobTitle.Host);
            PrintHelper.Print(fileName, printerName, documentName, printerSettings, data);
            //SetStatus("Print completed");
            Close();
        }

        /// <summary>
        /// Cancels print job
        /// </summary>
        /// <param name="printJob"></param>
        private void CancelJob(PrintJobData printJob)
        {
            SetStatus("Cancelling print job");
            //PrintHelper.CancelPrintJob(printJob);
            PrintHelper.CancelAllPrintJobs(printJob.PrintJobTitle, printJob.ServerHost);
            Close();
        }

        /// <summary>
        /// Performs print action
        /// </summary>
        private void PrintAction()
        {
            panelControl.Enabled = false;
            if (_printerSettings == null)
                GetChosenPrinterWithSettings(radioButtonColored.Checked);

            //CheckPrinterIssuesAndSend(comboBoxPrinters.SelectedText);
            PrintFirstTry(_psFileName, _printerName, _psFileData.Title, _printerSettings);
        }

        /// <summary>
        /// Gets chosen printer and its' settings
        /// </summary>
        /// <returns></returns>
        private Printer GetChosenPrinterWithSettings(bool colored)
        {
            try
            {
                LogHelper.LogDebug();

                Printer PrinterX = MainController.Singleton.AvailablePrinters.ByColor(colored)[comboBoxPrinters.SelectedIndex];

                _printerName = PrinterX.Name;

                if (PrinterX.Settings == null)
                {
                    _printerSettings = new PrinterSettings() {PrinterName = _printerName, Duplex = _printDuplex, DefaultPageSettings = {Color = colored}};
                    if (!_printerSettings.IsValid)
                        throw new NotSupportedException("Wrong printer settings");

                    PrinterX.Settings = _printerSettings;
                }
                else
                {
                    _printerSettings = PrinterX.Settings;
                    _printerSettings.Duplex = _printDuplex;
                    _printerSettings.DefaultPageSettings.Color = colored;
                    _printerSettings.Copies = _numberOfCopies;
                }

                return PrinterX;
            }
            catch (Exception ex)
            {
                Notifier.Error(ex);
            }

            return null;
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

            if (ConfigData.Config_DontCheckUserBalance)
            {

            }
            else
            {
                AllowedToPrintResponse res = APIWrapper.CheckIfCanPrint(Environment.MachineName, Environment.UserName, _printWithColor, _numberOfPrints, _printerName);
                if (res == null)
                {
                    Notifier.Warning(string.Format("No response from server but still printing."));
                    Print(_psFileName, _printerName, _psFileData.Title, _printerSettings, _psFileData);
                }
                else
                {
                    if (res.AllowedToPrint)
                    {
                        Print(_psFileName, _printerName, _psFileData.Title, _printerSettings, _psFileData);
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
        /// Tries to print first time 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="printerName"></param>
        /// <param name="printerSettings"></param>
        void PrintFirstTry(string fileName, string printerName, string documentName, PrinterSettings printerSettings)
        {
            if (ConfigData.Config_DontCheckUserBalance)
            {
                Print(fileName, printerName, documentName, printerSettings, _psFileData);
            }
            else
            {
                SetStatus("Checking user balance");
                var res = APIWrapper.CheckIfCanPrint(Environment.MachineName, Environment.UserName, printerSettings.DefaultPageSettings.Color, _numberOfPrints, printerName);
                if (res == null)
                {
                    Notifier.Warning(string.Format("No response from server but still printing."));
                    Print(fileName, printerName, documentName, printerSettings, _psFileData);
                }
                else
                {
                    if (res.Result)
                    {
                        if (res.AllowedToPrint)
                        {
                            Print(fileName, printerName, documentName, printerSettings, _psFileData);
                        }
                        else
                        {
                            //Notifier.Warning(string.Format("You are not allowed to print in the current time.{0}Reason: {1}", Environment.NewLine, res.Reason));
                            SetStatus("Not allowed to print");
                            if (Notifier.Question(string.Format("You are not allowed to print at the current time.{0}Reason: {1}{0}{2}", Environment.NewLine, res.Reason, "Do you want to leave the printing (Yes) or continue waiting (No)?")) == DialogResult.Yes)
                                Close();
                            else
                                StartTimerAllowedToPrint();
                        }
                    }
                    else
                    {
                        Notifier.Warning(string.Format("Not successful request CheckIfCanPrint."));
                    }
                }
            }
        }

        /// <summary>
        /// Starts timer
        /// </summary>
        private void StartTimerAllowedToPrint()
        {
            panelControl.Enabled = false;
            timerAllowedToPrint.Start();
        }

        /// <summary>
        /// Stops timer
        /// </summary>
        private void StopTimerAllowedToPrint()
        {
            timerAllowedToPrint.Stop();
            panelControl.Enabled = true;
        }

        /// <summary>
        /// Sets topmost for the form
        /// </summary>
        private void SetTopMost()
        {
#if (DEBUG)
            return;
#endif

            TopMost = true;
            TopMost = false;
            TopMost = true;
        }

        /// <summary>
        /// Changes printers and status if color option was changed
        /// </summary>
        private void ColorChanged()
        {
            ChangePrinters();

            SetStatus();
        }

        /// <summary>
        /// Gets duplex combo box index
        /// </summary>
        /// <param name="duplex"></param>
        /// <returns></returns>
        private int GetDuplexIndex(Duplex duplex)
        {
            //LogHelper.LogDebug("X Duplex " + duplex + " " + (int)duplex + " " + (int)Duplex.Simplex);
            return duplex == Duplex.Simplex ? 0 : duplex == Duplex.Vertical ? 1 : duplex == Duplex.Horizontal ? 2 : 3;
        }

        /// <summary>
        /// Sets special variables by printer settings
        /// </summary>
        private void SetByPrinterSettings()
        {
            try
            {
                if (_printerSettings != null)
                {
                    _printDuplex = _printerSettings.Duplex;
                    _printWithColor = _printerSettings.DefaultPageSettings.Color;
                    _numberOfCopies = _printerSettings.Copies;

                    comboBoxDuplex.SelectedIndex = GetDuplexIndex(_printDuplex);
                    radioButtonColored.Checked = _printWithColor;
                    radioButtonGrayScale.Checked = !_printWithColor;

                    if (!string.IsNullOrWhiteSpace(_printBooklet))
                    {
                        if (_printBooklet.Contains("2-sided"))
                            _printBooklet = null;
                        else
                        if (_printBooklet.Equals("None"))
                            _printBooklet = null;
                    }

                    LogHelper.LogDebug(_printBooklet);

                    _numberOfPrints = _psFileData.NumberOfPages;

                    if (!string.IsNullOrWhiteSpace(_printBooklet))
                    {
                        _numberOfPrints = (int)Math.Round(_numberOfPrints / 2.0, MidpointRounding.AwayFromZero);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            SetStatus();
        }

        #endregion

        /// Event Subscriptions
        #region Event Subscriptions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bPrint_Click(object sender, EventArgs e)
        {
            PrintAction();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonColored_CheckedChanged(object sender, EventArgs e)
        {
            ColorChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonGrayScale_CheckedChanged(object sender, EventArgs e)
        {
            ColorChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxPrinters_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetChosenPrinterWithSettings(radioButtonColored.Checked);
            //SetByPrinterSettings();
            SetStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDuplex_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxDuplex.SelectedIndex)
            {
                case 0: _printDuplex = Duplex.Simplex;
                    break;
                case 1: _printDuplex = Duplex.Vertical;
                    break;
                case 2: _printDuplex = Duplex.Horizontal;
                    break;
                case 4: _printDuplex = Duplex.Default;
                    break;
                default: _printDuplex = Duplex.Default;
                    break;
            }

            SetStatus();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintingControlForm_Load(object sender, EventArgs e)
        {
            SetTopMost();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintingControlForm_VisibleChanged(object sender, EventArgs e)
        {
            SetTopMost();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintingControlForm_Shown(object sender, EventArgs e)
        {
            GetDataAsync(_psFileName);

            SetTopMost();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bAdvancedSettings_Click(object sender, EventArgs e)
        {
            var pr = GetChosenPrinterWithSettings(radioButtonColored.Checked);

            UISettingsExtractor extractor = new UISettingsExtractor(_printerName, DesktopHelper.GetDesktopWindow());
            extractor.Start();

            MainController.Singleton.ShowAdvancedPrinterSettings(this, _printerName, ref _printerSettings);

            extractor.Stop();
            
            _printBooklet = extractor.Booklet;
            LogHelper.LogDebug(_printBooklet);

            pr.Settings = _printerSettings;
            SetByPrinterSettings();
        }
        
        #endregion
    }
}
