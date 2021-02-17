using System;
using System.Runtime.InteropServices;

namespace CubiclesPrinterLib.Win32.Print
{
    /// <summary>
    /// DEVMODE struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public Int32 dmFields;
        public short dmOrientation;
        public short dmPaperSize;
        public short dmPaperLength;
        public short dmPaperWidth;
        public short dmScale;
        public short dmCopies;
        public short dmDefaultSource;
        public short dmPrintQuality;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        public short dmUnusedPadding;
        public Int32 dmBitsPerPel;
        public Int32 dmPelsWidth;
        public Int32 dmPelsHeight;
        public Int32 dmNup;
        public Int32 dmDisplayFrequency;
        public Int32 dmICMMethod;
        public Int32 dmICMIntent;
        public Int32 dmMediaType;
        public Int32 dmDitherType;
        public Int32 dmReserved1;
        public Int32 dmReserved2;
        public Int32 dmPanningWidth;
        public Int32 dmPanningHeight;
    }
}
