using System;
using Cubicles.API.JsonClasses;
using Cubicles.Extensions;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cubicles.API
{
    /// <summary>
    /// This class represents Cubicles API Wrapper
    /// </summary>
    public class APIWrapper
    {
        /// GetPrinters
        #region GetPrinters

        /// <summary>
        /// Gets list of available printers
        /// </summary>
        /// <param name="machineName">computer name</param>
        /// <returns>list of available printers</returns>
        public static Printers GetPrinters(string machineName)
        {
            try
            {
                LogHelper.LogDebug(machineName);

                string res = API.GetPrinters(machineName);

                LogHelper.Log(res);

                if (string.IsNullOrWhiteSpace(res))
                    return null;

                return JsonConvert.DeserializeObject<Printers>(res);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// GetEnvironment
        #region GetEnvironment

        /// <summary>
        /// Gets environment data.
        /// </summary>
        /// <returns>environment data</returns>
        public static object GetEnvironment()
        {
            try
            {
                string res = API.GetEnvironment();

                LogHelper.Log(res);

                if (string.IsNullOrWhiteSpace(res))
                    return null;

                //return null;

                return JToken.Parse(res);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// CheckIfCanPrint
        #region CheckIfCanPrint

        /// <summary>
        /// Checks if the user is able to print
        /// </summary>
        /// <param name="machineName">computer name</param>
        /// <param name="userName">user name</param>
        /// <param name="isColored">is colored?</param>
        /// <param name="numberOfPrints">number of prints</param>
        /// <param name="printerName">printer name</param>
        /// <returns>is user able to print?</returns>
        public static AllowedToPrintResponse CheckIfCanPrint(string machineName, string userName, bool isColored, int numberOfPrints, string printerName)
        {
            try
            {
                LogHelper.LogDebug(machineName);

                var res = API.CheckIfCanPrint(machineName, userName, isColored, numberOfPrints, printerName);

                LogHelper.LogDebug(res);

                if (string.IsNullOrWhiteSpace(res))
                    return null;

                return JsonConvert.DeserializeObject<AllowedToPrintResponse>(res);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// LogPrint
        #region LogPrint

        /// <summary>
        /// Logs print entry data
        /// </summary>
        /// <param name="machineName">computer name</param>
        /// <param name="userName">user name</param>
        /// <param name="printerName">printer name</param>
        /// <param name="isColored">is colored?</param>
        /// <param name="numberOfPrints">number of prints</param>
        /// <param name="doubleSided">double-sided print</param>
        public static void LogPrint(string machineName, string userName, string printerName, bool isColored, int numberOfPrints, bool doubleSided)
        {
            try
            {
                LogHelper.LogDebug(machineName);

                var res = API.LogPrint(machineName, userName, isColored, numberOfPrints, printerName, doubleSided);

                LogHelper.LogDebug(res);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }

        #endregion

        /// PrinterIssue
        #region PrinterIssue

        /// <summary>
        /// Sends printer issue if any exist
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="issue">printer issue</param>
        public static void PrinterIssue(string machineName, string userName, string printerName, string issue)
        {
            try
            {
                LogHelper.LogDebug(printerName);

                var res = API.PrinterIssue(machineName, userName, printerName, issue);

                LogHelper.LogDebug(res);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }

        /// <summary>
        /// Sends printer issue if any exist
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="issue">printer issue</param>
        public static void PrinterIssue(string machineName, string userName, string printerName, PrinterTrouble issue)
        {
            API.PrinterIssue(machineName, userName, printerName, issue.Issue());
        }

        #endregion
    }
}
