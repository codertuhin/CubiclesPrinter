using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using CubiclesPrinterLib;
using CubiclesPrinterSetupCustomAction.Classes;
using Microsoft.Win32;

namespace CubiclesPrinterSetupCustomAction
{
    /// <summary>
    /// 
    /// </summary>
    public class SpoolerHelper
    {
        /// PInvoke Codes
        #region PInvoke Codes

        /// Printer Monitor
        #region Printer Monitor

        //API for Adding Print Monitors
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183341(v=vs.85).aspx
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddMonitor(String pName, UInt32 Level, ref MONITOR_INFO_2 pMonitors);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MONITOR_INFO_2
        {
            public string pName;
            public string pEnvironment;
            public string pDLLName;
        }

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 DeleteMonitor(String pName, String pEnvironment, String pMonitorName);

        #endregion

        /// Printer Port
        #region Printer Port

        /// <summary>
        /// 
        /// </summary>
        private const int MAX_PORTNAME_LEN = 64;

        /// <summary>
        /// 
        /// </summary>
        private const int MAX_NETWORKNAME_LEN = 49;
        private const int MAX_SNMP_COMMUNITY_STR_LEN = 33;
        private const int MAX_QUEUENAME_LEN = 33;
        private const int MAX_IPADDR_STR_LEN = 16;
        private const int RESERVED_BYTE_ARRAY_SIZE = 540;

        /// <summary>
        /// PrinterAccess
        /// </summary>
        private enum PrinterAccess
        {
            ServerAdmin = 0x01,
            ServerEnum = 0x02,
            PrinterAdmin = 0x04,
            PrinterUse = 0x08,
            JobAdmin = 0x10,
            JobRead = 0x20,
            StandardRightsRequired = 0x000f0000,
            PrinterAllAccess = (StandardRightsRequired | PrinterAdmin | PrinterUse)
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PrinterDefaults
        {
            public IntPtr pDataType;
            public IntPtr pDevMode;
            public PrinterAccess DesiredAccess;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct PortData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PORTNAME_LEN)]
            public string sztPortName;
        }

        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool OpenPrinter(string printerName, out IntPtr phPrinter, ref PrinterDefaults printerDefaults);
        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr phPrinter);
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool XcvDataW(IntPtr hXcv, string pszDataName, IntPtr pInputData, UInt32 cbInputData, out IntPtr pOutputData, UInt32 cbOutputData, out UInt32 pcbOutputNeeded, out UInt32 pdwStatus);

        #endregion

        /// Printer Driver
        #region Printer Driver

        //API for Adding Printer Driver
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183346(v=vs.85).aspx
        //http://pinvoke.net/default.aspx/winspool.DRIVER_INFO_2
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddPrinterDriver(String pName, UInt32 Level, ref DRIVER_INFO_3 pDriverInfo);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct DRIVER_INFO_3
        {
            public uint cVersion;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pEnvironment;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDriverPath;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDataFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pConfigFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pHelpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDependentFiles;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pMonitorName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pDefaultDataType;
        }

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern bool GetPrinterDriverDirectory(StringBuilder pName, StringBuilder pEnv, int Level, [Out] StringBuilder outPath, int bufferSize, ref int Bytes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pName"></param>
        /// <param name="pEnvironment"></param>
        /// <param name="pDriverName"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 DeletePrinterDriver(String pName, string pEnvironment, string pDriverName);

        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 DeletePrinterDriverEx(String pName, string pEnvironment, string pDriverName, uint wDeleteFlag, uint wVersionFlag);

        #endregion

        /// Printer
        #region Printer

        //API for Adding Printer
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd183343(v=vs.85).aspx
        [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern Int32 AddPrinter(string pName, uint Level, [In] ref PRINTER_INFO_2 pPrinter);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern bool DeletePrinter(IntPtr hPrinter);

        #endregion

        #endregion

        /// AddPrinterMonitor
        #region AddPrinterMonitor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorName"></param>
        /// <returns></returns>
        public GenericResult AddPrinterMonitor(string monitorName)
        {
            GenericResult retVal = new GenericResult("AddPrinterMonitor");
            MONITOR_INFO_2 mi2 = new MONITOR_INFO_2();

            mi2.pName = monitorName;
            mi2.pEnvironment = null;
            mi2.pDLLName = "mfilemon.dll";

            try
            {
                if (AddMonitor(null, 2, ref mi2) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }
        
        #endregion

        /// DeletePrinterMonitor
        #region DeletePrinterMonitor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorName"></param>
        /// <returns></returns>
        public GenericResult DeletePrinterMonitor(string monitorName)
        {
            LogHelper.Log("DeletePrinterMonitor Start.");
            GenericResult retVal = new GenericResult("DeletePrinterMonitor");

            try
            {
                String szMonitor = monitorName;

                LogHelper.Log("DeleteMonitor. " + monitorName);
                if (DeleteMonitor(null, null, monitorName) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }

            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;

            LogHelper.Log("DeletePrinterMonitor End. " + retVal.Success);
            return retVal;
        }

        #endregion

        /// AddPrinterPort
        #region AddPrinterPort

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="portType"></param>
        /// <returns></returns>
        public GenericResult AddPrinterPort(string portName, string portType)
        {
            GenericResult retVal = new GenericResult("AddPrinterPort");
            IntPtr printerHandle;
            PrinterDefaults defaults = new PrinterDefaults { DesiredAccess = PrinterAccess.ServerAdmin };
            try
            {
                if (!OpenPrinter(",XcvMonitor " + portType, out printerHandle, ref defaults))
                    throw new Exception("Could not open printer for the monitor port " + portType + "!");
                try
                {
                    PortData portData = new PortData { sztPortName = portName };
                    uint size = (uint)Marshal.SizeOf(portData);
                    IntPtr pointer = Marshal.AllocHGlobal((int)size);
                    Marshal.StructureToPtr(portData, pointer, true);
                    IntPtr outputData;
                    UInt32 outputNeeded, status;
                    try
                    {
                        if (!XcvDataW(printerHandle, "AddPort", pointer, size, out outputData, 0, out outputNeeded, out status))
                            retVal.Message = status.ToString();
                    }
                    catch (Exception ex)
                    {
                        retVal.Exception = ex;
                        retVal.Message = retVal.Exception.Message;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pointer);
                    }
                }
                catch (Exception ex)
                {
                    retVal.Exception = ex;
                    retVal.Message = retVal.Exception.Message;
                }

                finally
                {
                    ClosePrinter(printerHandle);
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        #endregion
        
        #region DeletePrinterPort

        public GenericResult DeletePrinterPort(string portName, string portType)
        {
            LogHelper.Log("DeletePrinterPort Start.");
            GenericResult retVal = new GenericResult("DeletePrinterPort");
            IntPtr printerHandle;
            PrinterDefaults defaults = new PrinterDefaults { DesiredAccess = PrinterAccess.ServerAdmin };
            try
            {
                if (!OpenPrinter(",XcvMonitor " + portType, out printerHandle, ref defaults))
                    throw new Exception("Could not open printer for the monitor port " + portType + "!");
                try
                {
                    PortData portData = new PortData { sztPortName = portName };
                    uint size = (uint)Marshal.SizeOf(portData);
                    IntPtr pointer = Marshal.AllocHGlobal((int)size);
                    Marshal.StructureToPtr(portData, pointer, true);
                    IntPtr outputData;
                    UInt32 outputNeeded, status;
                    try
                    {
                        if (!XcvDataW(printerHandle, "DeletePort", pointer, size, out outputData, 0, out outputNeeded, out status))
                            retVal.Message = status.ToString();
                    }
                    catch (Exception ex)
                    {
                        retVal.Exception = ex;
                        retVal.Message = retVal.Exception.Message;
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(pointer);
                    }
                }
                catch (Exception ex)
                {
                    retVal.Exception = ex;
                    retVal.Message = retVal.Exception.Message;
                }

                finally
                {
                    ClosePrinter(printerHandle);
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }

            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;

            LogHelper.Log("DeletePrinterPort End. " + retVal.Success);
            return retVal;
        }
        
        #endregion

        /// GetPrinterDirectory
        #region GetPrinterDirectory

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericResult GetPrinterDirectory()
        {
            GenericResult retVal = new GenericResult("GetPrinterDirectory");
            StringBuilder str = new StringBuilder(1024);
            int i = 0;
            GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
            try
            {
                GetPrinterDriverDirectory(null, null, 1, str, 1024, ref i);
                retVal.Success = true;
                retVal.Message = str.ToString();
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        #endregion

        /// AddPrinterDriver
        #region AddPrinterDriver

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverName"></param>
        /// <param name="driverPath"></param>
        /// <param name="dataPath"></param>
        /// <param name="configPath"></param>
        /// <param name="helpPath"></param>
        /// <returns></returns>
        GenericResult AddPrinterDriver(string driverName, string driverPath, string dataPath, string configPath, string helpPath)
        {
            GenericResult retVal = new GenericResult("AddPrinterDriver");
            DRIVER_INFO_3 di = new DRIVER_INFO_3();
            di.cVersion = 3;
            di.pName = driverName;
            di.pEnvironment = null;
            di.pDriverPath = driverPath;
            di.pDataFile = dataPath;
            di.pConfigFile = configPath;
            di.pHelpFile = helpPath;
            di.pDependentFiles = "";
            di.pMonitorName = null;
            di.pDefaultDataType = "RAW";

            try
            {
                if (AddPrinterDriver(null, 3, ref di) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean SetDefaultPrinter(String name);

        /// DeletePrinterDriver
        #region DeletePrinterDriver

        /// <summary>
        /// 
        /// </summary>
        /// <param name="driverName"></param>
        /// <returns></returns>
        public GenericResult DeletePrinterDriver(string driverName)
        {
            GenericResult retVal = new GenericResult("DeletePrinterDriver");

            try
            {
                if (DeletePrinterDriverEx(null, null, driverName, 4, 0) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;

            /*
            DRIVER_INFO_3 di = new DRIVER_INFO_3();
            di.cVersion = 3;
            di.pName = driverName;
            di.pEnvironment = null;
            di.pDriverPath = driverPath;
            di.pDataFile = dataPath;
            di.pConfigFile = configPath;
            di.pHelpFile = helpPath;
            di.pDependentFiles = "";
            di.pMonitorName = null;
            di.pDefaultDataType = "RAW";

            try
            {
                if (AddPrinterDriver(null, 3, ref di) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;*/
            return retVal;
        }

        #endregion

        /// AddPrinter
        #region AddPrinter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="portName"></param>
        /// <param name="driverName"></param>
        /// <returns></returns>
        public GenericResult AddPrinter(string printerName, string portName, string driverName)
        {
            GenericResult retVal = new GenericResult("AddPrinter");
            PRINTER_INFO_2 pi = new PRINTER_INFO_2();

            pi.pServerName = null;
            pi.pPrinterName = printerName;
            pi.pShareName = "";
            pi.pPortName = portName;
            pi.pDriverName = driverName;    // "Apple Color LW 12/660 PS";
            pi.pComment = "Use only Cubicles Printer";
            pi.pLocation = "";
            pi.pDevMode = new IntPtr(0);
            pi.pSepFile = "";
            pi.pPrintProcessor = "WinPrint";
            pi.pDatatype = "RAW";
            pi.pParameters = "";
            pi.pSecurityDescriptor = new IntPtr(0);

            try
            {
                if (AddPrinter(null, 2, ref pi) == 0)
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;
            return retVal;
        }

        #endregion

        /// DeletePrinter
        #region DeletePrinter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public GenericResult DeletePrinter(string printerName)
        {
            LogHelper.Log("DeletePrinter Start.");
            GenericResult retVal = new GenericResult("DeletePrinter");

            try
            {
                var pd = new PrinterDefaults();
                pd.DesiredAccess = PrinterAccess.PrinterAllAccess;
                IntPtr hPrinter;

                LogHelper.Log("Open Printer Start");
                if (OpenPrinter(printerName, out hPrinter, ref pd))
                {
                    LogHelper.Log("Open Printer Inner");
                    if (hPrinter != IntPtr.Zero)
                    {
                        LogHelper.Log("Delete Printer Inner");
                        if (!DeletePrinter(hPrinter))
                        {
                            retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                            retVal.Message = retVal.Exception.Message;
                        }

                        LogHelper.Log("Close Printer");
                        if (!ClosePrinter(hPrinter))
                        {
                            retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                            retVal.Message = retVal.Exception.Message;
                        }
                    }
                    LogHelper.Log("Open Printer End");
                }
                else
                {
                    retVal.Exception = new Win32Exception(Marshal.GetLastWin32Error());
                    retVal.Message = retVal.Exception.Message;
                }
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }

            if (string.IsNullOrWhiteSpace(retVal.Message))
                retVal.Success = true;

            LogHelper.Log("DeletePrinter End. " + retVal.Success);
            return retVal;
        }
        
        #endregion

        /// ConfigureVirtualPort
        #region ConfigureVirtualPort

        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitorName"></param>
        /// <param name="portName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public GenericResult ConfigureVirtualPort(string monitorName, string portName, string key)
        {
            GenericResult retVal = new GenericResult("ConfigureVirtualPort");
            try
            {
                //if (string.IsNullOrWhiteSpace(appPath))
                
                //string appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Cubicles\CubiclesPrinter");

                string appPath = ConfigData.Path_App;
                string outputPath = ConfigData.Path_Monitor;
                string filePattern = ConfigData.Pattern_File;
                //string filePattern = "%r_%c_%u_%Y%m%d_%H%n%s_%j.ps";
                string userCommand = string.Empty;
                var execPath = string.Empty;

                string keyName = string.Format(@"SYSTEM\CurrentControlSet\Control\Print\Monitors\{0}\{1}", monitorName, portName);
                Registry.LocalMachine.CreateSubKey(keyName);
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey(keyName, true);
                regKey.SetValue("OutputPath", outputPath, RegistryValueKind.String);
                regKey.SetValue("FilePattern", filePattern, RegistryValueKind.String);
                regKey.SetValue("Overwrite", 0, RegistryValueKind.DWord);
                regKey.SetValue("UserCommand", userCommand, RegistryValueKind.String);
                regKey.SetValue("ExecPath", execPath, RegistryValueKind.String);
                regKey.SetValue("WaitTermination", 0, RegistryValueKind.DWord);
                regKey.SetValue("PipeData", 0, RegistryValueKind.DWord);
                regKey.Close();
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }

            return retVal;
        }

        #endregion

        /// RestartSpoolService
        #region RestartSpoolService

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericResult RestartSpoolService()
        {
            GenericResult retVal = new GenericResult("RestartSpoolService");
            try
            {
                ServiceController sc = new ServiceController("Spooler");
                if (sc.Status != ServiceControllerStatus.Stopped || sc.Status != ServiceControllerStatus.StopPending)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                sc.Start();
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericResult StopSpoolService()
        {
            GenericResult retVal = new GenericResult("StoppoolService");
            try
            {
                ServiceController sc = new ServiceController("Spooler");
                if (sc.Status != ServiceControllerStatus.Stopped)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        public GenericResult StartSpoolService()
        {
            GenericResult retVal = new GenericResult("StartSpoolService");
            try
            {
                ServiceController sc = new ServiceController("Spooler");
                if (sc.Status != ServiceControllerStatus.Running || sc.Status != ServiceControllerStatus.StartPending)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
            }
            return retVal;
        }

        #endregion

        /// DeleteVirtualPrinter
        #region DeleteVirtualPrinter

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public GenericResult DeleteVirtualPrinter()
        {
            string printerName = ConfigData.PrinterName;
            string key = ConfigData.PrinterName;
            string monitorName = ConfigData.MonitorName;
            string portName = ConfigData.PortName;
            string driverName = ConfigData.DriverName;

            LogHelper.Log(string.Format("PrinterName: {0}", printerName));

            return _DeleteVirtualPrinter(printerName, key, monitorName, portName, driverName);
        }

        public GenericResult DeleteVirtualPrinter2()
        {
            string printerName = ConfigData.PrinterName2;
            string key = ConfigData.PrinterName2;
            string monitorName = ConfigData.MonitorName2;
            string portName = ConfigData.PortName2;
            string driverName = ConfigData.DriverName;

            LogHelper.Log(string.Format("PrinterName: {0}", printerName));

            return _DeleteVirtualPrinter(printerName, key, monitorName, portName, driverName);
        }

        public GenericResult DeleteVirtualPrinterOld()
        {
            string printerName = "Cubicles Printer";
            string key = "Cubicles Printer";
            string monitorName = ConfigData.MonitorName;
            string portName = "CubiclesPrinterPort:";
            string driverName = ConfigData.DriverName;

            LogHelper.Log(string.Format("PrinterName: {0}", printerName));

            return _DeleteVirtualPrinter(printerName, key, monitorName, portName, driverName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public GenericResult _DeleteVirtualPrinter(string printerName, string key, string monitorName, string portName, string driverName)
        {
            GenericResult retVal = new GenericResult("DeleteVirtualPrinter " + printerName);
            try
            {
                /*
                LogHelper.Log("Restarting Spool Service");
                GenericResult restartSpoolResult = RestartSpoolService();
                if (restartSpoolResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", restartSpoolResult.Exception));*/

                /*
                LogHelper.Log("Stopping Spool Service");
                GenericResult stopSpoolResult = StopSpoolService();
                if (stopSpoolResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", stopSpoolResult.Exception));*/

                //1 - Delete Printer
                LogHelper.Log("Deleting Printer");
                GenericResult printerResult = DeletePrinter(printerName);
                if (printerResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerResult.Exception.Message));

                //2 - Delete Printer Port
                LogHelper.Log("Deleting Printer Port.");
                GenericResult printerPortResult = DeletePrinterPort(portName, monitorName);
                if (printerPortResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerPortResult.Exception));

                //3 - Delete Printer Monitor
                LogHelper.Log("Deleting Printer Monitor.");
                GenericResult printerMonitorResult = DeletePrinterMonitor(monitorName);
                if (printerMonitorResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerMonitorResult.Exception));
                

                /*
                LogHelper.Log("Starting Spool Service");
                GenericResult startSpoolResult =StartSpoolService();
                if (startSpoolResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", startSpoolResult.Exception));*/

                /*
                //5 - Configure Virtual Port
                LogHelper.Log("Configuring Virtual Port");
                GenericResult configResult = ConfigureVirtualPort(monitorName, portName, key);
                if (configResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", configResult.Exception));*/
                /*
                //6 - Restart Spool Service
                LogHelper.Log("Restarting Spool Service");
                GenericResult restartSpoolResult = RestartSpoolService();
                if (restartSpoolResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", restartSpoolResult.Exception));*/

                LogHelper.Log("DeleteVirtualPrinter End");
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
                LogHelper.Log(string.Format("Exception: {0}", ex.Message));
            }

            return retVal;
        }

        #endregion

        /// AddVirtualPrinter

        #region AddVirtualPrinter

        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public GenericResult AddVirtualPrinter(string driverName)
        {
            string printerName = ConfigData.PrinterName;
            string key = ConfigData.PrinterName;
            string monitorName = ConfigData.MonitorName;
            string portName = ConfigData.PortName;
            //string portName = string.Format("{0}Port:", printerName);
            //string driverName = ConfigData.DriverName;

            return _AddVirtualPrinter(printerName, key, monitorName, portName, driverName);
        }

        public GenericResult AddVirtualPrinter2(string driverName)
        {
            string printerName = ConfigData.PrinterName2;
            string key = ConfigData.PrinterName2;
            string monitorName = ConfigData.MonitorName2;
            string portName = ConfigData.PortName2;
            //string portName = string.Format("{0}Port:", printerName);
            //string driverName = ConfigData.DriverName;

            return _AddVirtualPrinter(printerName, key, monitorName, portName, driverName);
        }

        public GenericResult AddPrinterDriver(string driverName, string dataFileName)
        {
            string driverFileName = "PSCRIPT5.DLL";
            //string dataFileName = "CubiclesPrinter.PPD";
            string configFileName = "PS5UI.DLL";
            string helpFileName = "PSCRIPT.HLP";

            string sysPath = Path.Combine(Environment.SystemDirectory, @"spool\drivers\w32x86");

            string driverPath = Path.Combine(sysPath, driverFileName);
            string dataPath = Path.Combine(sysPath, dataFileName);
            string configPath = Path.Combine(sysPath, configFileName);
            string helpPath = Path.Combine(sysPath, helpFileName);

            LogHelper.Log("Setting Driver Path and Files.");
            GenericResult printerDriverPath = GetPrinterDirectory();
            if (printerDriverPath.Success == true)
            {
                driverPath = string.Format("{0}\\{1}", printerDriverPath.Message, driverFileName);
                dataPath = string.Format("{0}\\{1}", printerDriverPath.Message, dataFileName);
                configPath = string.Format("{0}\\{1}", printerDriverPath.Message, configFileName);
                helpPath = string.Format("{0}\\{1}", printerDriverPath.Message, helpFileName);
            }

            LogHelper.Log("Adding Printer Driver.");
            //LogHelper.Log("driverPath : " + driverPath);
            GenericResult printerDriverResult = AddPrinterDriver(driverName, driverPath, dataPath, configPath, helpPath);
            if (printerDriverResult.Success == false)
                LogHelper.Log(string.Format("Exception: {0}", printerDriverResult.Exception));

            return printerDriverResult;
        }


        public GenericResult _AddVirtualPrinter(string printerName, string key, string monitorName, string portName, string driverName) 
        {
            LogHelper.Log("AddVirtualPrinter " + printerName);
            GenericResult retVal = new GenericResult("AddVirtualPrinter " + printerName);
            try
            {
                //1 - Add Printer Monitor
                LogHelper.Log("Adding Printer Monitor.");
                GenericResult printerMonitorResult = AddPrinterMonitor(monitorName);
                if (printerMonitorResult.Success == false)
                {
                    //if (printerMonitorResult.Message.ToLower() != "the specified print monitor has already been installed")
                    //throw printerMonitorResult.Exception;
                }

                //2 - Add Printer Port
                LogHelper.Log("Adding Printer Port.");
                GenericResult printerPortResult = AddPrinterPort(portName, monitorName);
                if (printerPortResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerPortResult.Exception));
                //throw printerPortResult.Exception;

                /*
                //3 - Add Printer Driver
                LogHelper.Log("Adding Printer Driver.");
                //LogHelper.Log("driverPath : " + driverPath);
                GenericResult printerDriverResult = AddPrinterDriver(driverName, driverPath, dataPath, configPath, helpPath);
                if (printerDriverResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerDriverResult.Exception));
                //throw printerDriverResult.Exception;*/

                //4 - Add Printer
                LogHelper.Log("Adding Printer");
                GenericResult printerResult = AddPrinter(printerName, portName, driverName);
                if (printerResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerResult.Exception.Message));
                //throw printerResult.Exception;

                //5 - Configure Virtual Port
                LogHelper.Log("Configuring Virtual Port");
                GenericResult configResult = ConfigureVirtualPort(monitorName, portName, key);
                if (configResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", configResult.Exception));
                //throw configResult.Exception;

                /*
                //6 - Restart Spool Service
                LogHelper.Log("Restarting Spool Service");
                GenericResult restartSpoolResult = RestartSpoolService();
                if (restartSpoolResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", restartSpoolResult.Exception));
                //throw restartSpoolResult.Exception;*/

                LogHelper.Log("AddVirtualPrinter End");
                retVal.Success = true;
            }
            catch (Exception ex)
            {
                retVal.Exception = ex;
                retVal.Message = retVal.Exception.Message;
                LogHelper.Log(string.Format("Exception: {0}", ex.Message));
            }

            return retVal;
        }

        #endregion

        
    }
}
