using System;
using System.Collections.Generic;
using CubiclesPrinterLib;
using CubiclesPrinterLib.Data;
using System.Windows;
using System.Windows.Threading;
using Cubicles.Extensions;
using Cubicles.Utility;
using CubiclesPrinterLib.Extensions;
using CubiclesPrinterUI.View;
using CubiclesPrinterUI.Model;
using Cubicles.API.JsonClasses;
using CubiclesPrinterUI.Controller;
using Cubicles.Utility.Helpers;

namespace CubiclesPrinterUI.ViewModel
{
    /// <summary>
    /// PrintingControlViewModel class.
    /// </summary>
    public sealed class PrintingControlViewModel : ViewModelBase, IDisposable
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Model </summary>
        private PrintingControlModel _model;

        /// <summary> Owner </summary>
        private PrintingControlWindow _owner;

        /// <summary> Flag if 'print colored' option enabled </summary>
        private bool _IsPrintColoredEnabled = false;

        #endregion

        /// Properties
        #region Properties

        /// <summary> Print job title </summary>
        public PrintJobTitle PrintJobTitle { get { return _model.Title; } set { OnPropertyChanged("PrintJobTitle"); } }

        /// <summary> PostScript file name</summary>
        public string PostScriptFileName { get { return _model.PSFileName; } set { OnPropertyChanged("PostScriptFileName"); } }

        /// <summary> PDF file name </summary>
        public string PDFFileName { get { return _model.PDFFileName; } set { OnPropertyChanged("PDFFileName"); } }

        /// <summary> PostScript metadata</summary>
        public PostScriptMetaData PostScriptMetaData { get { return _model.PostScriptMetaData; } set { OnPropertyChanged("PostScriptMetaData"); } }

        /// <summary> Number Of Prints </summary>
        public int NumberOfPrints { get { return _model.NumberOfPrints; } set { OnPropertyChanged("NumberOfPrints"); } }

        /// <summary> Number Of Pages </summary>
        public int NumberOfPages { get { return _model.NumberOfPages; } set { OnPropertyChanged("NumberOfPages"); } }

        /// <summary> Number Of Copies </summary>
        public int NumberOfCopies { get { return _model.NumberOfCopies; } set { OnPropertyChanged("NumberOfCopies"); } }

        /// <summary> Print Booklet </summary>
        public string PrintBooklet { get { return _model.PrintBooklet; } set { OnPropertyChanged("PrintBooklet"); } }

        /// <summary> Duplex </summary>
        public System.Drawing.Printing.Duplex PrintDuplex { get { return _model.PrintDuplex; } set { OnPropertyChanged("PrintDuplex"); } }

        /// <summary> </summary>
        public bool PrintBothSides { get { return _model.PrintDuplex == System.Drawing.Printing.Duplex.Simplex ? false : true; } set { _model.PrintDuplex = value ? System.Drawing.Printing.Duplex.Vertical : System.Drawing.Printing.Duplex.Simplex; _model.PrinterSettings.Duplex = _model.PrintDuplex; OnPropertyChanged("SelectedDuplexIndex"); OnPropertyChanged("PrintDuplex"); OnPropertyChanged("PrintJobStatus"); OnPropertyChanged("PrintBothSides");  } }

        /// <summary> SelectedDuplexIndex </summary>
        public int SelectedDuplexIndex { get { return _model.PrintDuplex.DuplexIndex(); } set { _model.PrintDuplex = value.DuplexFromIndex(); _model.PrinterSettings.Duplex = _model.PrintDuplex; OnPropertyChanged("SelectedDuplexIndex"); OnPropertyChanged("PrintDuplex"); OnPropertyChanged("PrintJobStatus"); OnPropertyChanged("PrintBothSides"); } }

        /// <summary> Printer Settings </summary>
        public System.Drawing.Printing.PrinterSettings PrinterSettings { get { return _model.PrinterSettings; } set { OnPropertyChanged("PrinterSettings"); } }

        /// <summary> Window Title </summary>
        public string Title { get { if (string.IsNullOrWhiteSpace(_model.Status)) return ConfigData.AppName; return string.Format("{0} : {1}", ConfigData.AppName, _model.Status); } set { OnPropertyChanged("Title"); } }

        /// <summary> Status </summary>
        public string Status { get { return _model.Status; } set { OnPropertyChanged("Status"); } }

        /// <summary> Printer status </summary>
        public string PrinterStatus{get{return string.Format("Status: {0} | Printer = {1}",Status,string.IsNullOrWhiteSpace(PrinterName) ? "Not chosen" : PrinterName);}set { OnPropertyChanged("PrinterStatus"); }}

        /// <summary> Printer name</summary>
        public string PrinterName { get { return _model.ChosenPrinter == null ? "Not chosen" : _model.ChosenPrinter.DisplayName; } set { OnPropertyChanged("PrinterName"); } }

        /// <summary> Document name</summary>
        public string DocumentName { get { return PostScriptMetaData == null ? (PrintJobTitle == null ? "Unknown" : (string.IsNullOrWhiteSpace(PrintJobTitle.Document) ? "Unknown" : PrintJobTitle.Document.Short())) : (string.IsNullOrWhiteSpace(PostScriptMetaData.Title) ? "Unknown" : PostScriptMetaData.Title.Short()); } set { OnPropertyChanged("DocumentName"); } }

        /// <summary> Print job status </summary>
        public string PrintJobStatus
        {
            get
            {
                return string.Format("Document = {1}{0}Number of prints = {2} | Has Color = {3} | Print Colored = {4} && Double-Sided = {5}{6}",
                    Environment.NewLine,
                    DocumentName,
                    NumberOfPrints,
                    PrintJobHasColor.HasValue ? (PrintJobHasColor.Value ? "Yes" : "No") : "Unknown",
                    PrintWithColor ? "Yes" : "No",
                    PrintDuplex == System.Drawing.Printing.Duplex.Default ? "Default" : PrintDuplex == System.Drawing.Printing.Duplex.Simplex ? "No" : "Yes",
                    string.IsNullOrWhiteSpace(PrintBooklet) ? "" : string.Format(" ( {0} )", PrintBooklet));
            }
            set { OnPropertyChanged("PrintJobStatus"); }
        }

        /// <summary> Print without color flag </summary>
        public bool PrintWithoutColor
        {
            get { return !_model.PrintWithColor; }
            set { PrintWithColor = !value; }
        }

        /// <summary> Print with color flag </summary>
        public bool PrintWithColor
        {
            get { return _model.PrintWithColor; }
            set
            {
                _model.PrintWithColor = value;
                LogHelper.LogDebug("Color changed; print with color : " + value);
                ChangePrinters();
                GetChosenPrinterWithSettings(PrintWithColor);

                OnPropertyChanged("PrintWithColor");
                OnPropertyChanged("PrintWithoutColor");
                OnPropertyChanged("PrintCost");
            }
        }

        /// <summary> Flag that print job has color </summary>
        public bool? PrintJobHasColor { get { return _model.FileHasColor; } set { OnPropertyChanged("PrintJobHasColor"); } }

        /// <summary> Flag if 'print colored' option enabled </summary>
        public bool IsPrintColoredEnabled { get { return IsBlockedMode ? false : _IsPrintColoredEnabled; } set { _IsPrintColoredEnabled = value; OnPropertyChanged("IsPrintColoredEnabled"); } }

        /// <summary> Flag if controls enabled (real printers mode) </summary>
        public bool IsControlsEnabled { get { return _model.IsControlsEnabled; } set { OnPropertyChanged("IsControlsEnabled"); OnPropertyChanged("IsSettingsEnabled"); OnPropertyChanged("SettingsText"); OnPropertyChanged("IsPrintersListEnabled"); OnPropertyChanged("IsDuplexListEnabled"); } }

        /// <summary> Flag if settings button enabled </summary>
        public bool IsSettingsEnabled { get { return _model.IsControlsEnabled; } set { OnPropertyChanged("IsSettingsEnabled"); OnPropertyChanged("SettingsText"); } }

        /// <summary> Settings button text </summary>
        public string SettingsText { get { return IsBlockedMode ? "Change Settings" : "Advanced Settings"; } set { OnPropertyChanged("SettingsText"); } }

        /// <summary> Flag if printers list enabled </summary>
        public bool IsPrintersListEnabled { get { return IsBlockedMode ? false : _model.IsControlsEnabled; } set { OnPropertyChanged("IsPrintersListEnabled"); } }

        /// <summary> Flag if duplex list enabled </summary>
        public bool IsDuplexListEnabled { get { return IsBlockedMode ? false : _model.IsControlsEnabled; } set { OnPropertyChanged("IsDuplexListEnabled"); } }

        /// <summary> Selected Printer </summary>
        public Printer SelectedPrinter
        {
            get
            {
                return _model.ChosenPrinter;
            }
            set
            {
                // set chosen printer
                _model.ChosenPrinter = value;

                // check chosen printer
                if (_model.ChosenPrinter == null)
                    // set index of chosen printer to 0
                    _model.SelectedPrinterIndex = 0;                
                else
                    // set index of chosen printer
                    _model.SelectedPrinterIndex = Printers.ByColor(_model.ChosenPrinter.IsColored).IndexOf(_model.ChosenPrinter);

                if (_model.ChosenPrinter != null)
                {
                    LogHelper.Log("Printer changed to " + _model.ChosenPrinter.ToString());
                    if (_model.ChosenPrinter.Settings == null)
                        _model.ChosenPrinter.Settings = new System.Drawing.Printing.PrinterSettings() { PrinterName = _model.ChosenPrinter.Name };

                    _model.PrinterSettings = _model.ChosenPrinter.Settings;
                }          

                OnPropertyChanged("SelectedPrinter");
            }
        }

        /// <summary> List of available printers </summary>
        public Printers Printers
        {
            get
            {
                // check if real printer chosen
                if (IsBlockedMode)
                {
                    _model.SelectedPrinterIndex = 0;
                    return new Printers() { _model.ChosenPrinter };
                }

                LogHelper.LogDebug("List of available printers UI : " + _model.Printers.Count);

                return _model.Printers;
            }
            set { OnPropertyChanged("Printers"); OnPropertyChanged("SelectedPrinterIndex"); OnPropertyChanged("SelectedPrinter"); }
        }

        /// <summary> Duplex options list </summary>
        public List<string> DuplexOptions { get { return Extensions.DuplexOptions; } set { OnPropertyChanged("DuplexOptions"); } }

        /// <summary> Selected Printer Index </summary>
        public int SelectedPrinterIndex
        {
            get
            {
                // check if blocked mode then we have nothing to show (or show only predefined line)
                if (IsBlockedMode)
                    return 0;

                return _model.SelectedPrinterIndex;
            }
            set
            {
                _model.SelectedPrinterIndex = value;                         

                // get printer by color option
                GetChosenPrinterWithSettings(PrintWithColor);

                // update UI
                OnPropertyChanged("SelectedPrinterIndex");
                OnPropertyChanged("SelectedPrinter");
            }
        }

        /// <summary> Is blocked mode </summary>
        public bool IsBlockedMode { get { return _model.IsBlockedMode; } set { OnPropertyChanged("IsBlockedMode"); } }

        /// <summary> User PC </summary>
        public string UserPC { get { return _model.UserPC; } set { OnPropertyChanged("UserPC"); } }

        /// <summary> Color Page Cost </summary>
        public int ColorPageCost { get { return _model.ColorPageCost; } set { OnPropertyChanged("ColorPageCost"); } }

        /// <summary> Black And White Page Cost </summary>
        public int BlackAndWhitePageCost { get { return _model.BlackAndWhitePageCost; } set { OnPropertyChanged("BlackAndWhitePageCost"); } }

        /// <summary> Print Cost </summary>
        public int PrintCost { get { return (PrintWithColor ? ColorPageCost : BlackAndWhitePageCost) * NumberOfPrints; } set { OnPropertyChanged("PrintCost"); } }

        #endregion  

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owner">owner</param>
        /// <param name="model">model</param>
        public PrintingControlViewModel(PrintingControlWindow owner, PrintingControlModel model)
        {
            // show preparing progress
            // MainController.Singleton.ShowProgressWindow("Preparing", "Your document is being prepared for printing, please wait...");

            // set owner
            _owner = owner;

            // set model
            _model = model;
            _model.Closed += _model_Closed;
            _model.StatusChanged += _model_StatusChanged;
            _model.ColorDetermined += _model_ColorDetermined;
            _model.ReadyToPrint += _model_ReadyToPrint;

            // init model
            model.Init();

            // set top most
            SetTopMost();
        }
        
        #endregion

        /// Model Event Handling
        #region Model Event Handling

        /// <summary>
        /// Processes ReadyToPrint event of the model.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        void _model_ReadyToPrint(object sender, EventArgs e)
        {
            LogHelper.LogDebug("ReadyToPrint EventHandling");
            if (_owner != null)
            {
                _owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    LogHelper.LogDebug("ReadyToPrint Invoke");
                    // check the mode
                    if (!_model.IsBlockedMode)
                        if (_model.FileHasColor.HasValue && _model.FileHasColor.Value && _model.IsColorVirtualPrinterSelected)
                            PrintWithColor = true;

                    SetUIByColor();
                    //SelectedPrinterIndex = _model.SelectedPrinterIndex;
                    Update();
                    LogHelper.LogDebug("Close ProgressWindow");
                    MainController.Singleton.CloseProgressWindow();
                }), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Processes ColorDetermined event of the model. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _model_ColorDetermined(object sender, EventArgs e)
        {
            if (_owner != null)
            {
                _owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Update();
                }), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Processes StatusChanged event of the model. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _model_StatusChanged(object sender, System.EventArgs e)
        {
            if (_owner != null)
            {
                _owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Update();
                }), DispatcherPriority.Background);
            }
        }

        /// <summary>
        /// Processes Closed event of the model. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_Closed(object sender, System.EventArgs e)
        {
            Close();
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Shows advanced settings.
        /// </summary>
        public void ShowAdvancedSettings()
        {
            // check the mode
            if (IsBlockedMode)
            {
                ShowBlockedModeChangesWarning();
            }
            else
            {
                // show advanced settings
                _model.ShowAdvancedSettings(_owner);

                // update UI
                Update();
            }
        }

        /// <summary>
        /// Shows blocked mode changes warning.
        /// </summary>
        private void ShowBlockedModeChangesWarning()
        {
            if (WPFNotifier.Question(string.Format("To make changes you have to get back to your document, make changes and then click 'Print' button once again.{0}{0}Do you want to abort and quit?", Environment.NewLine)) == MessageBoxResult.Yes)
            {
                Close();
            }
        }

        /// <summary>
        /// Prints / Resumes print job.
        /// </summary>
        public void Print()
        {
            // check window
            if (_owner != null)
            {
                _owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _owner.Hide();
                }), DispatcherPriority.Background);
            }

            MainController.Singleton.ShowProgressWindow("Sending", "Your document is being sent to the printer. \r\n\r\nPlease make sure the document has been fully printed before you log out.");

            // check the model and chosen printer
            if (_model != null && _model.ChosenPrinter != null)
            {
                // check the settings of the chosen printer
                if (_model.ChosenPrinter.Settings == null)
                {
                    WPFNotifier.Error("Default printer settings will be set");
                    _model.ChosenPrinter.Settings = new System.Drawing.Printing.PrinterSettings() { PrinterName = _model.ChosenPrinter.Name };
                }

                _model.ChosenPrinter.Settings.DefaultPageSettings.Color = PrintWithColor;

                // print / resume
                _model.Print();
            }
            else
                WPFNotifier.Error("Can't print - printer aren't chosen");

            // update UI
            Update();
        }

        /// <summary>
        /// Checks for closing.
        /// </summary>
        public void CheckClosing()
        {
            if (_model != null)
                if (!_model.IsPrintActionPerformed)
                    _model.Cancel(); 
        }

        /// <summary>
        /// Chanes printers.
        /// </summary>
        public void ChangePrinters()
        {
            // change printers
            _model.ChangePrinters();

            // update UI
            Update();
        }

        /// <summary>
        /// Sets UI by selected color option
        /// </summary>
        private void SetUIByColor()
        {
            if (!_model.CanPrintWithColor)
            {
                IsPrintColoredEnabled = false;
                return;
            }

            if (!_model.FileHasColor.HasValue)
            {
                IsPrintColoredEnabled = false;
            }
            else
            {
                if (_model.FileHasColor.Value)
                {
                    IsPrintColoredEnabled = true;
                }
                else
                {
                    IsPrintColoredEnabled = false;
                }
            }
        }

        /// <summary>
        /// Sets topmost for the form
        /// </summary>
        private void SetTopMost()
        {
#if (!DEBUG)
            if (_owner != null)
            {
                _owner.Topmost = true;
                _owner.Topmost = false;
                _owner.Topmost = true;
            }
#endif
            MainController.Singleton.SetProgressWindowTopMost();
        }

        /// <summary>
        /// Updates UI visuals.
        /// </summary>
        public void Update()
        {
            OnPropertyChanged("DocumentName");
            OnPropertyChanged("PrintWithColor");
            OnPropertyChanged("PrintWithoutColor");
            OnPropertyChanged("PrintCost");
            OnPropertyChanged("NumberOfPages");
            OnPropertyChanged("NumberOfPrints");
            OnPropertyChanged("PrinterName");
            OnPropertyChanged("PrintJobTitle");
            OnPropertyChanged("PrintJobData");
            OnPropertyChanged("PrintJobHasColor");
            OnPropertyChanged("PostScriptFileName");
            OnPropertyChanged("Title");
            OnPropertyChanged("Status");
            OnPropertyChanged("PrinterStatus");
            OnPropertyChanged("PrintJobStatus");
            OnPropertyChanged("PrintDuplex");
            OnPropertyChanged("PrintBooklet");
            OnPropertyChanged("PrintWithColor");
            OnPropertyChanged("Printers");
            OnPropertyChanged("SelectedPrinter");
            OnPropertyChanged("SelectedPrinterIndex");
            OnPropertyChanged("SelectedDuplexIndex");
            OnPropertyChanged("IsControlsEnabled");
            OnPropertyChanged("IsSettingsEnabled");
            OnPropertyChanged("SettingsText");
            OnPropertyChanged("IsPrintersListEnabled");
            OnPropertyChanged("IsDuplexListEnabled");

            SetTopMost();
        }

        /// <summary>
        /// Gets chosen printer by color.
        /// </summary>
        /// <param name="colored">color printer flag</param>
        private void GetChosenPrinterWithSettings(bool colored)
        {
            // get chosen printer
            _model.GetChosenPrinterWithSettings(colored);

            // update UI
            Update();
        }

        /// <summary>
        /// Updates print job data with the new data.
        /// </summary>
        /// <param name="data">new data</param>
        public void UpdateData(PrintJobData data)
        {
            // update data
            _model.UpdateData(data);

            // update UI
            Update();
        }

        /// <summary>
        /// Closes window.
        /// </summary>
        private void Close()
        {
            // check window
            if (_owner != null)
            {
                try
                {
                    _owner.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        MainController.Singleton.CloseProgressWindow();
                        // close the window
                        _owner.Close();
                    }
                    catch { }
                }), DispatcherPriority.Background);
                }
                catch { }
            }
        }

        /// <summary>
        /// Selects recommended printer by color.
        /// </summary>
        public void SelectRecommendedPrinter()
        {
            _model.SelectRecommendedPrinter(PrintWithColor);

            // update UI
            Update();
        }  

        /// <summary>
        /// Disposes data.
        /// </summary>
        public void Dispose()
        {
            // dispose model
            if (_model != null)
            {
                _model.Closed -= _model_Closed;
                _model.StatusChanged -= _model_StatusChanged;
                _model.ColorDetermined -= _model_ColorDetermined;
                _model.ReadyToPrint -= _model_ReadyToPrint;
                _model = null;
            }

            // dispose owner
            if (_owner != null)
            {
                _owner = null;
            }
        }

        #endregion
    }
}
