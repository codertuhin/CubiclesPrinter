using System;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows.Automation;
using Cubicles.Utility.Helpers;

namespace CubiclesPrinterLib.Utility
{
    /// <summary>
    /// Advanced Printer Settings Extractor fron the Settings Dialog
    /// </summary>
    public class UISettingsExtractor
    {
        /// Private Variables
        #region Private Variables

        /// <summary> Update timer </summary>
        private Timer timerUpdate;

        /// <summary> Printer name </summary>
        private string _printerName;

        /// <summary> Handle </summary>
        private IntPtr _handle;
        
        #endregion

        /// Properties
        #region Properties

        /// <summary> Booklet value </summary>
        public string Booklet { get; private set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="handle">andle</param>
        public UISettingsExtractor(string printerName, IntPtr handle)
        {
            _printerName = printerName;
            _handle = handle;

            // initialize timer
            timerUpdate = new Timer(500);
            timerUpdate.AutoReset = true;
            timerUpdate.Enabled = false;
            timerUpdate.Elapsed += timerUpdate_Elapsed;

            LogHelper.LogDebug(_printerName);
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Starts extraction process,
        /// </summary>
        public void Start()
        {
            LogHelper.LogDebug();
            if (timerUpdate != null)
                timerUpdate.Start();
        }

        /// <summary>
        /// Stops extraction process.
        /// </summary>
        public void Stop()
        {
            LogHelper.LogDebug();
            if (timerUpdate != null)
                timerUpdate.Stop();
        }

        /// <summary>
        /// Resets values.
        /// </summary>
        public void Reset()
        {
            Booklet = null;
        }

        /// <summary>
        /// Extracts data from Brother printer.
        /// </summary>
        /// <param name="handle">handle</param>
        private void ExtractDataBrother(IntPtr handle)
        {
            try
            {
                // get desktop window
                AutomationElement desktop = AutomationElement.FromHandle(handle);
                if (desktop == null)
                    return;

                // get all windows of current process id
                AutomationElementCollection windows = desktop.FindAll(TreeScope.Children, new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window),
                    new PropertyCondition(AutomationElement.ProcessIdProperty, Process.GetCurrentProcess().Id)));

                // check windows
                if (windows == null || windows.Count < 1)
                    return;
                
                AutomationElement dialog = null;

                // iterate through all the windows and find settings dialog
                foreach (AutomationElement window in windows)
                {
                    string name = window.Current.Name;
                    if (name.Contains(_printerName))
                        dialog = window;
                    else if (name.Replace(" ", "").Contains(_printerName.Replace(" ", "")))
                        dialog = window;
                    else if (!name.Contains("Printing Control"))
                        dialog = window;

                    if (dialog != null)
                        break;
                }

                // check the dialog
                if (dialog == null)
                    return;

                // get booklet combo box
                AutomationElement comboBoxBooklet = dialog.FindFirst(TreeScope.Children, new AndCondition(
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox),
                    new PropertyCondition(AutomationElement.NameProperty, "2-sided / Booklet")));

                // check the combo box
                if (comboBoxBooklet == null)
                    return;

                // extract data
                var data = ((SelectionPattern) comboBoxBooklet.GetCurrentPattern(SelectionPattern.Pattern)).Current.GetSelection().First().Current.Name;
                if (data != null)
                    Booklet = data.ToString();
            }
            catch (ElementNotAvailableException)
            {
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timerUpdate_Elapsed(object sender, ElapsedEventArgs e)
        {
            timerUpdate.Enabled = false;

            switch (_printerName)
            {
                case "Canon MG3500 series Printer":
                    ExtractDataBrother(_handle);
                    break;
                default:
                    ExtractDataBrother(_handle);
                    break;
            }

            timerUpdate.Enabled = true;
        }
        
        #endregion
    }
}
