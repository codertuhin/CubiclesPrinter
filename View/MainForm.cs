using System;
using System.Drawing;
using System.Windows.Forms;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinter.Controller;
using CubiclesPrinterLib;

namespace CubiclesPrinter.View
{
    /// <summary>
    /// This class represent main form of the app
    /// </summary>
    public partial class MainForm : Form
    {
        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            Init();
        }

        /// <summary>
        /// Initializes UI and logic
        /// </summary>
        private void Init()
        {
            try
            {
                HideX();

                MainController.Singleton.MainForm = this;
                MainController.Singleton.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        /// <summary>
        /// Hides form from alt-tab
        /// </summary>
        private void HideX()
        {
            if (ConfigData.Config_ShowUI)
                return;

            try
            {
                //Opacity = 0;
                Visible = false;
                FormBorderStyle = FormBorderStyle.FixedToolWindow;
                ShowInTaskbar = false;
                Location = new Point(-Size.Width * 2, -Size.Height * 2);
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
            
        }

        #endregion

        /// Event Subscriptions
        #region Event Subscriptions

        /// <summary>
        /// on startup, hide our window and remove ourselves from the taskbar
        /// </summary>
        /// <param Name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            HideX();
            base.OnLoad(e);
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bShowPrintControl_Click(object sender, EventArgs e)
        {
            MainController.Singleton.LaunchPrintControl(@"Test.ps", false);
        }

        #endregion

        /// CLOSE ISSUE on Shut Down
        #region CLOSE ISSUE

        /// <summary>
        /// 
        /// </summary>
        private int WM_QUERYENDSESSION = 0x11;
        private bool systemShutdown = false;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg.Equals(WM_QUERYENDSESSION))
            {
                systemShutdown = true;
            }

            // If this is WM_QUERYENDSESSION, the closing event should be fired in the base WndProc
            base.WndProc(ref m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (systemShutdown)
            {
                // reset the variable since they may cancel the shutdown
                systemShutdown = false;
                e.Cancel = false;
            }

            CloseX();
        }

        /// <summary>
        /// 
        /// </summary>
        private void CloseX()
        {
            MainController.Singleton.Dispose();

            try
            {
                //Environment.Exit(0);
                Application.ExitThread();
            }
            catch (Exception ex)
            {
                ex = ex;
            }
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bCheckPrinterStatus_Click(object sender, EventArgs e)
        {
            //Notifier.Notify(MainController.Singleton.CheckPrinters());
        }

        /// <summary>
        /// Test method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonJobStatus_Click(object sender, EventArgs e)
        {
            //Notifier.Notify(MainController.Singleton.CheckJob());
            //Notifier.Notify(MainController.Singleton.CheckQueue());
        }
        
        #endregion
    }
}
