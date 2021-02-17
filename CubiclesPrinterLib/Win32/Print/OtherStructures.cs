using System;
using System.Runtime.InteropServices;

namespace CubiclesPrinterLib.Win32.Print
{
    /// <summary>
    /// POINTL struct
    /// </summary>
    public struct POINTL
    {
        public Int32 x;
        public Int32 y;
    }

    /// <summary>
    /// PRINTER_DEFAULTS struct
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PRINTER_DEFAULTS
    {
        public Int32 pDatatype;
        public Int32 pDevMode;
        [MarshalAs(UnmanagedType.U4)]
        public uint DesiredAccess;
    }
}

