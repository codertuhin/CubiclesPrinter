using System;
using System.Diagnostics;
using System.IO;
using CubiclesPrinterLib;
using CubiclesPrinterSetupCustomAction.Classes;

namespace CubiclesPrinterSetupCustomAction.Helpers
{
    public class InstallHelper
    {
        /// Uninstall
        #region Uninstall

        /// <summary>
        /// 
        /// </summary>
        public static void Uninstall()
        {
            try
            {
                File.Delete(ConfigData.FilePath_UninstallLog);
            }
            catch
            {
            }

            LogHelper.LogFileName = ConfigData.FilePath_UninstallLog;
            LogHelper.Log("Uninstall Started.");

            ProcessHelper.KillProcess(Path.GetFileNameWithoutExtension(ConfigData.AppExeName));

            string driverName = ConfigData.DriverName;
            GenericResult result = null;
            
            try
            {
                result = new SpoolerHelper().DeleteVirtualPrinter();
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("DeleteVirtualPrinter", ex.Message, ex);
            }

            try
            {
                result = new SpoolerHelper().DeleteVirtualPrinter2();
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("DeleteVirtualPrinter2", ex.Message, ex);
            }

            
            try
            {
                LogHelper.Log("Stopping Spool Service");
                result = new SpoolerHelper().StopSpoolService();
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("StopSpoolService", ex.Message, ex);
            }
            

            try
            {
                LogHelper.Log("Deleting Printer Driver.");
                GenericResult printerDriverResult = new SpoolerHelper().DeletePrinterDriver(driverName);
                if (printerDriverResult.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", printerDriverResult.Exception));
            }
            catch (Exception ex)
            {
                LogError("DeletePrinterDriver", ex.Message, ex);
            }

            try
            {
                LogHelper.Log("Starting Spool Service");
                result = new SpoolerHelper().StartSpoolService();
                if (result.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", result.Exception));
            }
            catch (Exception ex)
            {
                LogError("StartSpoolService", ex.Message, ex);
            }

            /*
            try
            {
                LogHelper.Log("Restarting Spool Service");
                result = new SpoolerHelper().RestartSpoolService();
                if (result.Success == false)
                    LogHelper.Log(string.Format("Exception: {0}", result.Exception));
            }
            catch (Exception ex)
            {
                LogError("RestartSpoolService", ex.Message, ex);
            }*/

            AutorunHelper.RemoveFromStartup();

            /*
            try
            {
                if (Directory.Exists(ConfigData.Path_App))
                {
                    DirectoryInfo di = new DirectoryInfo(ConfigData.Path_App);
                    di.Delete(true);
                }
            }
            catch (Exception ex)
            {
                LogError("Folder deletion ", ex.Message, ex);
            }
            LogHelper.Log("Uninstall Finished.");*/
        }
        
        #endregion

        /// Install
        #region Install

        /// <summary>
        /// 
        /// </summary>
        public static void Install()
        {
            try
            {
                File.Delete(ConfigData.FilePath_InstallLog);
            }
            catch
            {
            }

            LogHelper.LogFileName = ConfigData.FilePath_InstallLog;

            ProcessHelper.KillProcess(Path.GetFileNameWithoutExtension(ConfigData.AppExeName));

            LogHelper.Log("Install Started.");

            string driverName = ConfigData.DriverName;
            GenericResult result = null;

            try
            {
                LogHelper.Log("Adding Printer Driver");
                result = new SpoolerHelper().AddPrinterDriver(driverName, "CubiclesPrinterBlack.PPD");
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("AddPrinterDriver", ex.Message, ex);
            }

            try
            {
                result = new SpoolerHelper().AddVirtualPrinter(driverName);
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("AddVirtualPrinter", ex.Message, ex);
            }

            string driverName2 = ConfigData.DriverName2;

            try
            {
                LogHelper.Log("Adding Printer Driver");
                result = new SpoolerHelper().AddPrinterDriver(driverName2, "CubiclesPrinter.PPD");
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("AddPrinterDriver", ex.Message, ex);
            }

            try
            {
                LogHelper.Log("Install " + driverName2);
                result = new SpoolerHelper().AddVirtualPrinter2(driverName2);
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("AddVirtualPrinter2", ex.Message, ex);
            }
            
            try
            {
                LogHelper.Log("Restarting Spool Service");
                result = new SpoolerHelper().RestartSpoolService();
                if (result.Success == false)
                    LogError(result.Method, result.Message, result.Exception);
            }
            catch (Exception ex)
            {
                LogError("RestartSpoolService", ex.Message, ex);
            }

            try
            {
                LogHelper.Log("Setting Default Printer " + ConfigData.PrinterName);
                SpoolerHelper.SetDefaultPrinter(ConfigData.PrinterName);
            }
            catch (Exception ex)
            {
                LogError("SetDefaultPrinter", ex.Message, ex);
            }

            AutorunHelper.AddToStartup();
            LogHelper.Log("Install Finished.");
        }
        
        #endregion

        /// Commit
        #region Commit

        /// <summary>
        /// 
        /// </summary>
        public void Commit()
        {
            LogHelper.Log("Commit.");
            LogHelper.Log(ConfigData.Path_AppExe);

            try
            {
                Process.Start(ConfigData.Path_AppExe);
            }
            catch (Exception ex)
            {
                LogError("Commit Start Process Error", ex.Message, ex);
            }

            LogHelper.Log("Commit Completed.");
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exceptionSource"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        private static void LogError(string exceptionSource, string message, Exception innerException)
        {
            string eventMessage = string.Format("Source: {0}\nMessage: {1}\nInnerException: {2}", exceptionSource, message, innerException);
            LogHelper.Log(eventMessage);
        }

        #endregion
    }
}
