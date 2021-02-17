using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Management;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Data;
using CubiclesPrinterLib.Helpers;

namespace CubiclesPrinterLib.Extensions
{
    /// <summary>
    /// This class contains extension methods
    /// </summary>
    public static class Extensions
    {
        /// String
        #region String

        /// <summary>
        /// Gets full path from the relative one and places it inside the specified directory.
        /// </summary>
        /// <param name="fileName">full file path</param>
        /// <param name="path">new outer directory</param>
        /// <returns>full path inside the specified directory</returns>
        public static string FullPathFromRelative(this string fileName, string path)
        {
            // get root
            string root = Path.GetPathRoot(fileName);

            LogHelper.LogDebug(root);

            // check root
            if (string.IsNullOrEmpty(root))
                // we can create full path now
                return Path.Combine(path, fileName);

            return fileName;
        }

        #endregion

        /// Duplex
        #region Duplex

        /// <summary>
        /// Gets duplex index inside te list of duplex options.
        /// </summary>
        /// <param name="duplex">duplex</param>
        /// <returns>index</returns>
        public static int DuplexIndex(this Duplex duplex)
        {
            return duplex == Duplex.Simplex ? 0 : duplex == Duplex.Vertical ? 1 : duplex == Duplex.Horizontal ? 2 : 3;
        }

        /// <summary> Duplex options </summary>
        private readonly static List<string> _duplexOptions = new List<string>() { "Single", "Duplex Vertical", "Duplex Horizontal", "Default" };

        /// <summary> List of duplex options </summary>
        public static List<string> DuplexOptions { get { return _duplexOptions; } }

        /// <summary>
        /// Gets duplex value from the index of duplex options.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>duplex</returns>
        public static Duplex DuplexFromIndex(this int index)
        {
            switch (index)
            {
                case 0: return Duplex.Simplex;
                case 1: return Duplex.Vertical;
                case 2: return Duplex.Horizontal;
                case 3: return Duplex.Default;
                default: return Duplex.Default;
            }
        }

        #endregion

        /// ManagementBaseObject
        #region ManagementBaseObject

        /// <summary>
        /// Gets print job status from the specified management object
        /// </summary>
        /// <param name="printJob">print job</param>
        /// <returns>print job status</returns>
        public static PrintJobStatus JobStatus(this ManagementBaseObject printJob)
        {
            if (printJob == null)
                return new PrintJobStatus(PrintJobStatusEnum.None, null);

            string JobStatusString;
            if (printJob.Properties["JobStatus"] == null)
                return new PrintJobStatus(PrintJobStatusEnum.None, null);
            
            if (printJob.Properties["JobStatus"].Value == null)
                return new PrintJobStatus(PrintJobStatusEnum.None, null);

            PrintJobStatus JobStatus = new PrintJobStatus(PrintJobStatusEnum.None, null);
            JobStatusString = printJob.Properties["JobStatus"].Value.ToString();
            if (!string.IsNullOrWhiteSpace(JobStatusString))
            {
                //LogHelper.LogDebug(JobStatusString);
                if (JobStatusString.Contains("Spooling"))
                    JobStatus.JobStatus |= PrintJobStatusEnum.Spooling;
                if (JobStatusString.Contains("Printing"))
                    JobStatus.JobStatus |= PrintJobStatusEnum.Printing;
                if (JobStatusString.Contains("Paused"))
                    JobStatus.JobStatus |= PrintJobStatusEnum.Paused;
                if (JobStatusString.Contains("Deleting"))
                    JobStatus.JobStatus |= PrintJobStatusEnum.Deleting;
                if (JobStatusString.Contains("Error"))
                    JobStatus.JobStatus |= PrintJobStatusEnum.Error;
            }
            else
            {
                JobStatus.JobStatus = PrintJobStatusEnum.None;
            }

            JobStatus.JobStatusString = JobStatusString;

            return JobStatus;
        }

        #endregion

        /// Image
        #region Image

        /// <summary>
        /// Indicates whether the image has color or not
        /// </summary>
        /// <param name="value">image to be inspected</param>
        /// <returns>true if has color; otherwise false</returns>
        public static bool HasColor(this Image value)
        {
            return ImageHelper.HasColor(value);
        }

        #endregion
    }
}
