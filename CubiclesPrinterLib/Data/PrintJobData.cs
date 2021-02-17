using System;
using System.Drawing.Printing;
using System.Management;
using CubiclesPrinterLib.Extensions;
using CubiclesPrinterLib.Helpers;

namespace CubiclesPrinterLib.Data
{
    /// <summary>
    /// This class represents print job data
    /// </summary>
    public class PrintJobData
    {
        /// Properties
        #region Properties

        /// <summary> Print job management object </summary>
        public ManagementBaseObject PrintJob { get; private set; }

        /// <summary> Print Job Title </summary>
        public PrintJobTitle PrintJobTitle { get; set; }

        /// <summary> Host / system name </summary>
        public string ServerHost { get; set; }

        /// <summary> Flag indicates whether the job colored or not </summary>
        public bool Color { get; private set; }

        /// <summary> Print duplex </summary>
        public Duplex Duplex { get; private set; }

        /// <summary> Number of copies </summary>
        public short NumberOfCopies { get; set; }

        /// <summary> Print job status </summary>
        public PrintJobStatus JobStatus { get; private set; }

        /// <summary> Printer name </summary>
        public string RemotePrinterName
        {
            get
            {
                if (PrintJobTitle == null)
                    return null;

                return string.Format(PrintJobTitle.PrinterName);
            }
        }

        #endregion

        /// Init
        #region Init
        
        /// <summary>
        /// Constructor
        /// </summary>
        public PrintJobData()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printJob">management base object as a print job</param>
        /// <param name="host">host name</param>
        public PrintJobData(ManagementBaseObject printJob, string host)
        {
            if (printJob == null)
                return;

            ServerHost = host;
            PrintJob = printJob;
            PrintJobTitle = new PrintJobTitle();

            try
            {
                JobStatus = new PrintJobStatus(PrintJobStatusEnum.None, null);

                string name;
                int id;

                // extract printer name and job Id
                PrintHelper.ExtractNameAndId(printJob, out name, out id);

                PrintJobTitle.PrinterName = name;
                PrintJobTitle.JobID = id;
                PrintJobTitle.Document = printJob.Properties["Document"].Value.ToString();
                PrintJobTitle.Host = printJob.Properties["HostPrintQueue"].Value.ToString();
                if (!string.IsNullOrWhiteSpace(PrintJobTitle.Host))
                    PrintJobTitle.Host = PrintJobTitle.Host.Replace("\\", "").Trim();

                PrintJobTitle.Owner = printJob.Properties["Owner"].Value.ToString();

                Color = false;
                string color = printJob.Properties["Color"].Value.ToString();
                if (!string.IsNullOrWhiteSpace(color))
                    if (color.ToLower() != "monochrome")
                        Color = true;

                JobStatus = printJob.JobStatus();

                int totalPages = -1;
                int.TryParse(printJob.Properties["TotalPages"].Value.ToString(), out totalPages);
                PrintJobTitle.TotalPages = totalPages;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Wrong PrintJobData", ex);
            }
        }
        
        #endregion
    }
}
