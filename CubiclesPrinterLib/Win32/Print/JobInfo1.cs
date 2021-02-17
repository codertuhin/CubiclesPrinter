using System;
using System.Runtime.InteropServices;

namespace CubiclesPrinterLib.Win32.Print
{
    /// <summary>
    /// JOB_INFO_1 struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode)]
    public struct JOB_INFO_1
    {
        public UInt32 JobId;
        public string pPrinterName;
        public string pMachineName;
        public string pUserName;
        public string pDocument;
        public string pDatatype;
        public string pStatus;
        public UInt32 Status;
        public UInt32 Priority;
        public UInt32 Position;
        public UInt32 TotalPages;
        public UInt32 PagesPrinted;
        public SYSTEMTIME Submitted;
    }
}
