using System;
using System.Runtime.InteropServices;

namespace CubiclesPrinterLib.Win32.Print
{
    /// <summary>
    /// JOB_INFO_2 struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode), System.Security.SuppressUnmanagedCodeSecurity()]
    public class JOB_INFO_2
    {
        public UInt32 JobId;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrinterName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pMachineName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pUserName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDocument;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pNotifyName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDatatype;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pPrintProcessor;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pParameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pDriverName;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 LPDeviceMode;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string pStatus;
        public Int32 lpSecurity;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 Status;// PrintJobStatuses
        public UInt32 Priority;
        public UInt32 Position;
        public UInt32 StartTime;
        public UInt32 UntilTime;
        public UInt32 TotalPages;
        public UInt32 JobSize;
        [MarshalAs(UnmanagedType.Struct)]
        public SYSTEMTIME Submitted;
        public UInt32 Time;
        public UInt32 PagesPrinted;

        public DEVMODE dmOut;
    }
}
