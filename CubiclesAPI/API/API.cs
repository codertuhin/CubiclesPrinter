using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Cubicles.API
{
    /// <summary>
    /// This class contains API requests to server
    /// </summary>
    public sealed class API
    {
        /// URLs
        #region URLs

        /// <summary> The main URL </summary>
        private static string _url = @"http://api:52323/api/";

        /// <summary> The GetPrinters URL </summary>
        private static string _url_GetPrinters = _url + @"GetPrinters";

        /// <summary> The GetEnvironment URL </summary>
        private static string _url_GetEnvironment = _url + @"GetEnvironment";

        /// <summary> The CheckIfCanPrint URL </summary>
        private static string _url_CheckIfCanPrint = _url + @"Print";

        /// <summary> The LogPrint URL </summary>
        private static string _url_LogPrint = _url + @"LogPrint";

        /// <summary> The PrinterIssue URL  </summary>
        private static string _url_PrinterIssue = _url + @"PrinterIssue";

        /// <summary> The Test URL  </summary>
        private static string _url_Test = _url + @"Test";

        #endregion

        /// LoadConfig
        #region LoadConfig

        /// <summary>
        /// Loads API config data
        /// </summary>
        /// <param name="fileName"></param>
        public static string LoadConfig(string fileName)
        {
            LogHelper.LogDebug();

            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            try
            {
                if (!File.Exists(fileName))
                    return null;

                string text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                string[] sstext = text.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                if (sstext == null || sstext.Length < 1)
                    return null;

                LogHelper.LogDebug("API config : " + fileName);
                StringBuilder sb = new StringBuilder();
                foreach (string line in sstext)
                {
                    string[] liness = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                    if (liness == null || liness.Length != 2)
                        continue;

                    string key = liness[0].Trim();
                    string value = liness[1].Trim();
                    switch (key)
                    {
                        case "API_MainURL":
                            _url = value;
                            sb.AppendLine(line);
                            break;
                        case "API_GetPrinters":
                            _url_GetPrinters = value;
                            sb.AppendLine(line);
                            break;
                        case "API_GetEnvironment":
                            _url_GetEnvironment = value;
                            sb.AppendLine(line);
                            break;
                        case "API_CheckIfCanPrint":
                            _url_CheckIfCanPrint = value;
                            sb.AppendLine(line);
                            break;
                        case "API_LogPrint":
                            _url_LogPrint = value;
                            sb.AppendLine(line);
                            break;
                        case "API_PrinterIssue":
                            _url_PrinterIssue = value;
                            sb.AppendLine(line);
                            break;
                    }
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// Test
        #region Test

        /// <summary>
        /// Test request
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns></returns>
        public static JObject Test()
        {
            LogHelper.LogDebug();

            IRestResponse response = Connection.Get(_url_Test);
            if (response == null)
                return null;

            if (string.IsNullOrWhiteSpace(response.Content))
                return null;

            JObject json = JObject.Parse(response.Content);
            if (json == null)
                return null;

            return json;
        }

        #endregion

        /// GetPrinters
        #region GetPrinters

        /// <summary>
        /// GetPrinters request
        /// </summary>
        /// <param name="machineName">machine name</param>
        /// <returns>printers</returns>
        public static string GetPrinters(string machineName)
        {
            LogHelper.LogDebug();

            Dictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                {"workstation", machineName},
            };

            LogHelper.LogDebug(_url_GetPrinters);

            IRestResponse response = Connection.Get(_url_GetPrinters, queryParameters);
            if (response == null)
                return null;

            return response.Content;
        }

        #endregion

        /// GetEnvironment
        #region GetEnvironment

        /// <summary>
        /// Performs GetEnvironment request.
        /// </summary>
        /// <returns>environment data</returns>
        public static string GetEnvironment()
        {
            LogHelper.LogDebug();

            LogHelper.LogDebug(_url_GetEnvironment);

            IRestResponse response = Connection.Get(_url_GetEnvironment);
            if (response == null)
                return null;

            return response.Content;
        }

        #endregion

        /// CheckIfCanPrint
        #region CheckIfCanPrint

        /// <summary>
        /// CheckIfCanPrint request
        /// </summary>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        /// <param name="isColored">color option</param>
        /// <param name="numberOfPrints">number of prints</param>
        /// <param name="printerName">printer name</param>
        /// <returns>response result</returns>
        public static string CheckIfCanPrint(string machineName, string userName, bool isColored, int numberOfPrints, string printerName)
        {
            LogHelper.LogDebug();

            Dictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                {"workstation", machineName},
                {"username", userName},
                {"isColor", isColored.ToString()},
                {"numberOfPrints", numberOfPrints.ToString()},
                {"printerName", printerName}
            };

            LogHelper.LogDebug(_url_CheckIfCanPrint);

            IRestResponse response = Connection.Get(_url_CheckIfCanPrint, queryParameters);
            if (response == null)
                return null;

            return response.Content;
        }

        #endregion

        /// LogPrint
        #region LogPrint

        /// <summary>
        /// LogPrint request
        /// </summary>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        /// <param name="isColored">color option</param>
        /// <param name="numberOfPrints">number of prints</param>
        /// <param name="printerName">printer name</param>
        /// <param name="doubleSided">double sided print</param>
        /// <returns>response result</returns>
        public static string LogPrint(string machineName, string userName, bool isColored, int numberOfPrints, string printerName, bool doubleSided)
        {
            LogHelper.LogDebug(machineName);

            Dictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                {"workstation", machineName},
                {"username", userName},
                {"isColor", isColored.ToString()},
                {"numberOfPrints", numberOfPrints.ToString()},
                {"printerName", printerName},
                {"doubleSided", doubleSided.ToString()}
            };

            LogHelper.LogDebug(_url_LogPrint);

            IRestResponse response = Connection.Post(_url_LogPrint, queryParameters);
            if (response == null)
                return null;

            return response.Content;
        }

        #endregion

        /// PrinterIssue
        #region PrinterIssue

        /// <summary>
        /// PrinterIssue request
        /// </summary>
        /// <param name="machineName">machine name</param>
        /// <param name="userName">user name</param>
        /// <param name="printerName">printer name</param>
        /// <param name="issue">printer issue</param>
        /// <returns>response result</returns>
        public static string PrinterIssue(string machineName, string userName, string printerName, string issue)
        {
            LogHelper.LogDebug(printerName);

            Dictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                {"printerName", printerName},
                {"workstation", machineName},
                {"username", userName},
                {"issue", issue},
            };

            LogHelper.LogDebug(_url_PrinterIssue);

            IRestResponse response = Connection.Post(_url_PrinterIssue, queryParameters);
            if (response == null)
                return null;

            return response.Content;
        }

        #endregion
    }
}
