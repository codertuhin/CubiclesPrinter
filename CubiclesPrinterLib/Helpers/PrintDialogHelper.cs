using Cubicles.Utility;
using System;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// This class contains print dialog
    /// </summary>
    public class PrintDialogHelper
    {
        /// Win32
        #region Win32

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hMem"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr GlobalLock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalUnlock(IntPtr hMem);
        [DllImport("kernel32.dll")]
        static extern bool GlobalFree(IntPtr hMem);
        [DllImport("winspool.Drv", EntryPoint = "DocumentPropertiesW", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int DocumentProperties(IntPtr hwnd, IntPtr hPrinter, [MarshalAs(UnmanagedType.LPWStr)] string pDeviceName, IntPtr pDevModeOutput, IntPtr pDevModeInput, int fMode);

        private const int DM_PROMPT = 4;
        private const int DM_OUT_BUFFER = 2;
        private const int DM_IN_BUFFER = 8;

        #endregion

        /// EditPrinterSettings
        #region EditPrinterSettings

        /// <summary>
        /// Shows Edit Printer Settings dialog
        /// </summary>
        /// <param name="printerSettings">printer settings</param>
        /// <param name="handle">dialog owner handle</param>
        /// <returns>dialog result</returns>
        public static DialogResult EditPrinterSettings(PrinterSettings printerSettings, IntPtr handle)
        {
            DialogResult myReturnValue = DialogResult.Cancel;

            // get handle to devmode
            IntPtr hDevMode = printerSettings.GetHdevmode(printerSettings.DefaultPageSettings);

            // lock data
            IntPtr pDevMode = GlobalLock(hDevMode);

            // get properties
            int sizeNeeded = DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, pDevMode, pDevMode, 0);
            IntPtr devModeData = Marshal.AllocHGlobal(sizeNeeded);

            // show dialog
            long userChoice = DocumentProperties(handle, IntPtr.Zero, printerSettings.PrinterName, devModeData, pDevMode, DM_IN_BUFFER | DM_PROMPT | DM_OUT_BUFFER);
            long IDOK = (long)DialogResult.OK;

            // set and save settings from dialog
            if (userChoice == IDOK)
            {
                myReturnValue = DialogResult.OK;
                printerSettings.SetHdevmode(devModeData);
                printerSettings.DefaultPageSettings.SetHdevmode(devModeData);
            }

            // unlock data and free memory
            GlobalUnlock(hDevMode);
            GlobalFree(hDevMode);
            Marshal.FreeHGlobal(devModeData);

            return myReturnValue;
        }

        #endregion
    }
}
