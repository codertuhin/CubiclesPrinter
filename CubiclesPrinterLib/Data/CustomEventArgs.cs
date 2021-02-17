using System;
using Cubicles.API.JsonClasses;

namespace CubiclesPrinterLib.Data
{
    /// <summary>
    /// This class represent print job data event args
    /// </summary>
    public class PrintJobDataEventArgs : EventArgs
    {
        /// Properties
        #region Properties

        /// <summary> Data of the event </summary>
        public PrintJobData Data { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public PrintJobDataEventArgs(PrintJobData data)
        {
            Data = data;
        }

        #endregion
    }

    /// <summary>
    /// This class represent printer issue event args
    /// </summary>
    public class PrinterIssueEventArgs : EventArgs
    {
        /// Properties
        #region Properties

        /// <summary> Printer name </summary>
        public string PrinterName { get; set; }

        /// <summary> Printer issue </summary>
        public PrinterTrouble Issue { get; set; }

        /// <summary> Printer status </summary>
        public string Status { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="issue">issue</param>
        /// <param name="status">status</param>
        public PrinterIssueEventArgs(string printerName, PrinterTrouble issue, string status)
        {
            PrinterName = printerName;
            Issue = issue;
            Status = status;
        }

        #endregion
    }
}
