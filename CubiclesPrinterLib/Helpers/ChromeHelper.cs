using Cubicles.Utility.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// ChromeHelper class contains methods for working with Google Chrome.
    /// </summary>
    public class ChromeHelper
    {
        /// Reset Printer
        #region Reset Printer

        /// <summary>
        /// Resets last printer used in Google Chrome.
        /// </summary>
        public static void ResetLastPrinterUsed()
        {
            try
            {
                // compose full file path to the Chrome Preferences file
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\User Data\Default\Preferences");

                // check file name
                if (string.IsNullOrWhiteSpace(fileName))
                    return;

                // check the existence of the file
                if (!File.Exists(fileName))
                    return;

                // extract data from Preferences file
                string data = File.ReadAllText(fileName);

                // check the extracted data
                if (string.IsNullOrWhiteSpace(data))
                    return;

                // extract json Chrome Preferences
                var json = JObject.Parse(data);
                // check json
                if (json != null)
                {
                    // extract json data from 'printing'
                    var json_printing = json["printing"];
                    // check json
                    if (json_printing != null)
                    {
                        // extract json data from 'print_preview_sticky_settings'
                        var json_printing_print_preview_sticky_settings = json_printing["print_preview_sticky_settings"];
                        // check json
                        if (json_printing_print_preview_sticky_settings != null)
                        {
                            // replace last used printer with an empty json
                            json_printing["print_preview_sticky_settings"] = "{}";
                        }
                    }
                }

                // write data back to the Chrome Preferences file
                File.WriteAllText(fileName, json.ToString());
            }
            catch(Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        #endregion
    }
}
