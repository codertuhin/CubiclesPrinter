using System;
using System.Runtime.InteropServices;

namespace CubiclesPrinterLib.Win32.Print
{
    /// <summary>
    /// PrintJob class
    /// </summary>
    public class PrintJob
    {
        /// Native Method Imports
        #region Native Method Imports
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPrinterName"></param>
        /// <param name="phPrinter"></param>
        /// <param name="pDefault"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter(String pPrinterName, out IntPtr phPrinter, Int32 pDefault);

        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", EntryPoint = "GetJob", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetJob(IntPtr hPrinter, Int32 dwJobId, Int32 Level, IntPtr lpJob, Int32 cbBuf, ref Int32 lpbSizeNeeded);

        [DllImport("winspool.drv", EntryPoint = "GetJob", SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = false, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetJob(Int32 hPrinter, Int32 dwJobId, Int32 Level, IntPtr lpJob, Int32 cbBuf, ref Int32 lpbSizeNeeded);

        [DllImport("winspool.drv", EntryPoint = "SetJobA")]
        public static extern bool SetJob(IntPtr hPrinter, int JobId, int Level, IntPtr pJob, int Command_Renamed);

        private const int DM_ORIENTATION = 0x1;

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Gets the copy count of the specified printer's job id.
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="jobId">job id</param>
        /// <returns>copy count</returns>
        public static int GetCopyCount(string printerName, int jobId)
        {
            try
            {
                // get job info
                JOB_INFO_2 ji = GetJobInfo(printerName, jobId);

                // get copy count
                int copy = ji.dmOut.dmCopies;

                ji = null;

                return copy;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets printer handle.
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <returns>handle</returns>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        private static IntPtr GetPrinterHandle(string printerName)
        {
            IntPtr _printerHandle = IntPtr.Zero;
            PRINTER_DEFAULTS pDefaults = new PRINTER_DEFAULTS();

            pDefaults.pDatatype = 0;
            pDefaults.pDevMode = 0;
            pDefaults.DesiredAccess = 0;

            try
            {
                OpenPrinter(printerName, out _printerHandle, 0);
                return _printerHandle;
            }
            catch
            {
                try
                {
                    pDefaults.DesiredAccess = 0xf000c;
                    return _printerHandle;
                }
                catch
                {
                    try
                    {
                        pDefaults.DesiredAccess = 0x00000008 | 0x00020000; // PRINTER_ACCESS_USE Or READ_CONTROL
                        return _printerHandle;
                    }
                    catch
                    {
                    }
                }
            }

            return _printerHandle;
        }


        /// <summary>
        /// Gets job info (2) of the specified printer's job id.
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="jobId">job id</param>
        /// <returns>job info (2)</returns>
        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static JOB_INFO_2 GetJobInfo(string printerName, int jobId)
        {
            try
            { 
                IntPtr _printerHandle = IntPtr.Zero;

                // get printer handle
                _printerHandle = GetPrinterHandle(printerName);
                if (_printerHandle == IntPtr.Zero)
                    return null;
                
                //Get the JOB details from GetJob()
                JOB_INFO_2 jobInfo = GetJobInfo2(_printerHandle, jobId);

                // close printer
                bool res = ClosePrinter(_printerHandle);

                // return job info
                return jobInfo;
            }
            catch (AccessViolationException)
            {
            }
            catch (Exception)
            {
            }

            return null;
        }

        /// <summary>
        /// Gets job info (2) of the specified printer's job id.
        /// </summary>
        /// <param name="_printerHandle">printer handle</param>
        /// <param name="jobId">job id</param>
        /// <returns>job info (2)</returns>
        private static JOB_INFO_2 GetJobInfo2(IntPtr _printerHandle, int jobId)
        {
            try
            {

                JOB_INFO_2 info = new JOB_INFO_2();
                Int32 BytesWritten = default(Int32);
                IntPtr ptBuf = default(IntPtr);

                //Get the required buffer size
                if (!GetJob(_printerHandle, jobId, 2, ptBuf, 0, ref BytesWritten))
                {
                    if (BytesWritten == 0)
                    {
                        throw new Exception("GetJob for JOB_INFO_2 failed on handle: " + _printerHandle.ToString() + " for job: " + jobId);
                    }
                }

                // Allocate a buffer the right size
                if (BytesWritten > 0)
                    ptBuf = Marshal.AllocHGlobal(BytesWritten * 2);

                if (!GetJob(_printerHandle, jobId, 2, ptBuf, BytesWritten, ref BytesWritten))
                {
                    Marshal.FreeHGlobal(ptBuf);
                    throw new Exception("GetJob for JOB_INFO_2 failed on handle: " + _printerHandle.ToString() + " for job: " + jobId);
                }
                else
                {
                    info = new JOB_INFO_2();
                    Marshal.PtrToStructure(ptBuf, info);

                    //Fill the devmode structure
                    IntPtr ptrDevMode = new IntPtr(info.LPDeviceMode);
                    Marshal.PtrToStructure(ptrDevMode, info.dmOut);
                }

                // Free the allocated memory
                Marshal.FreeHGlobal(ptBuf);

                // return job info
                return info;
            }
            catch (AccessViolationException)
            {
            }
            catch (Exception)
            {
            }

            return null;
        }

        #endregion
    }
}
