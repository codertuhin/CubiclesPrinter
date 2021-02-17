using System;

namespace CubiclesPrinterLib.Data
{
    /// <summary>
    /// Enumeration of print job statuses
    /// </summary>
    [Flags]
    public enum PrintJobStatusEnum
    {
        None = 0,
        Spooling = 1,
        Printing = 2,
        Paused = 4,
        Deleting = 8,
        Error = 16,
        Completed = 32,
        Cancelled = 64
    }

    /// <summary>
    /// PrintJobStatus class
    /// </summary>
    public class PrintJobStatus
    {
        /// Properties
        #region Properties

        /// <summary> Job Status </summary>
        public PrintJobStatusEnum JobStatus { get; set; }

        /// <summary> Job Status String </summary>
        public string JobStatusString { get; set; }

        #endregion

        /// Init
        #region Init

        /// <summary> 
        /// Constructor 
        /// </summary>
        public PrintJobStatus()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="jobStatus"></param>
        /// <param name="jobStatusString"></param>
        public PrintJobStatus(PrintJobStatusEnum jobStatus, string jobStatusString)
        {
            JobStatus = jobStatus;
            JobStatusString = jobStatusString;
        }

        #endregion

        /// Override
        #region Override

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JobStatusString;
        }

        #endregion
    }
}
