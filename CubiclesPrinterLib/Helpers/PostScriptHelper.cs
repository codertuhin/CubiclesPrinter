using System;
using System.IO;
using System.Linq;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Data;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// This class allows to interact with postscript file
    /// </summary>
    public sealed class PostScriptHelper
    {
        /// GetData
        #region GetData

        /// <summary>
        /// Gets postscript file meta data
        /// </summary>
        /// <param name="fileName">file to be inspected</param>
        /// <returns>postscript file meta data</returns>
        public static PostScriptMetaData GetData(string fileName)
        {
            return PostScriptMetaData.ReadFile(fileName);
        }
        
        #endregion

        /// HasColor
        #region HasColor

        /// <summary>
        /// Checks whether the postscript file has color inside
        /// </summary>
        /// <param name="psFileName">file to be inspected</param>
        /// <returns>true if colored; otherwise false</returns>
        public static bool? HasColor(string psFileName)
        {
            if (string.IsNullOrWhiteSpace(psFileName))
                return null;

            LogHelper.LogDebug();

            try
            {
                if (!File.Exists(psFileName))
                    return null;

                string pdfTMPFileName = Path.GetTempFileName() + ".pdf";

                // and convert PS file to PDF
                GhostScriptHelper.ConvertPStoPDF(psFileName, pdfTMPFileName);

                // find out do PDF has color or not?
                bool hasColor = GhostScriptHelper.HasColor(pdfTMPFileName);

                try
                {
                    File.Delete(pdfTMPFileName);
                }
                catch (Exception ex)
                {
                    LogHelper.Log(ex);
                }

                return hasColor;
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// IsPS
        #region IsPS

        /// <summary>
        /// Checks if the file is a postscript file
        /// </summary>
        /// <param name="fileName">file to be inspected</param>
        /// <returns>true if postscript file; otherwise false</returns>
        public static bool IsPS(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            try
            {
                if (!File.Exists(fileName))
                    return false;

                // get lines
                string[] line = File.ReadLines(fileName).ToArray();
                if (line == null || line.Length < 1)
                    return false;

                // check the first line
                if (line[0].Contains(@"%!PS-"))
                    return true;

                if (line[0].Contains(@"@PJL"))
                    return true;

                /*
                // get the first line
                string line = File.ReadLines(fileName).ToArray()[0];
                if (string.IsNullOrWhiteSpace(line))
                    return false;

                // check the first line
                if (line.Contains(@"%!PS-"))
                    return true;*/
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }

            return false;
        }

        #endregion
    }
}
