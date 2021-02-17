using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Extensions;
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using Ghostscript.NET.Rasterizer;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// This class contains helper methods for work with ghostscript
    /// </summary>
    public class GhostScriptHelper
    {
        /// GetLib
        #region GetLib

        /// <summary>
        /// Gets proper library depending on 32 or 64 bit architecture
        /// </summary>
        /// <returns>path to the library</returns>
        private static string getLib(bool fullPath)
        {
            if (!fullPath)
            {
                if (Environment.Is64BitProcess)
                    return @"x64\gsdll64.dll";

                return @"x86\gsdll32.dll";
            }

            if (Environment.Is64BitProcess)
                    return Path.Combine(ConfigData.Path_App, @"x64\gsdll64.dll");
            
            return Path.Combine(ConfigData.Path_App, @"x86\gsdll32.dll");
        }

        /// <summary>
        /// Gets proper library
        /// </summary>
        /// <returns>path to the library</returns>
        private static string GetLib()
        {
            try
            {
                string path = getLib(true);
                if (File.Exists(path))
                    return path;

                path = getLib(false);
                if (File.Exists(path))
                    return path;

                path = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), getLib(false));
                if (File.Exists(path))
                    return path;

                throw new ArgumentNullException("File not found! " + Environment.NewLine + Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        #endregion

        /// Args
        #region Args

        /// <summary>
        /// Gets arguments for postscript to pdf conversion 
        /// </summary>
        /// <param name="inputPath">postscript file</param>
        /// <param name="outputPath">pdf file</param>
        /// <returns>arguments</returns>
        private static string[] ConvertPStoPDFArgs(string inputPath, string outputPath)
        {
            List<string> gsArgs = new List<string>();

            gsArgs.Add("-q");
            gsArgs.Add("-dSAFER");
            gsArgs.Add("-dBATCH");
            gsArgs.Add("-dNOPAUSE");
            gsArgs.Add("-dNOPROMPT");
            gsArgs.Add("-sDEVICE=pdfwrite");
            gsArgs.Add("-r720");
            //gsArgs.Add("-sPAPERSIZE=a4");
            gsArgs.Add("-dNumRenderingThreads=" + Environment.ProcessorCount.ToString());
            //gsArgs.Add("-dTextAlphaBits=4");
            //gsArgs.Add("-dGraphicsAlphaBits=4");
            gsArgs.Add(@"-sOutputFile=" + outputPath);
            gsArgs.Add(@"-f" + inputPath);

            return gsArgs.ToArray();
        }

        #endregion

        /// HasColor
        #region HasColor

        /// <summary>
        /// Determines if the specified file has color
        /// </summary>
        /// <param name="fileName">file to be inspected</param>
        /// <returns>true if has color; otherwise false</returns>
        public static bool HasColor(string fileName, int desired_x_dpi = 96, int desired_y_dpi = 96)
        {
            LogHelper.LogDebug(fileName);
            try
            {
                GhostscriptVersionInfo gsVersion = new GhostscriptVersionInfo(GetLib());
                using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
                {
                    rasterizer.Open(fileName, gsVersion, false);

                    for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                    {
                        try
                        {
                            using (Image img = rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber))
                            {
                                if (img.HasColor())
                                {
                                    LogHelper.LogDebug("Has Color");
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogDebug(ex);
                        }
                    }

                    rasterizer.Close();
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }

            LogHelper.LogDebug("No Color");
            return false;
        }

        #endregion

        /// Convert
        #region Convert

        /// <summary>
        /// Converts postscript file to pdf
        /// </summary>
        /// <param name="psFileName">postscript file name</param>
        /// <returns>pdf file name</returns>
        public static void ConvertPStoPDF(string psFileName, string pdfFileName)
        {
            if (string.IsNullOrWhiteSpace(psFileName))
                return;

            if (string.IsNullOrWhiteSpace(pdfFileName))
                return;

            try
            {
                GhostscriptProcessor process = CreateProcessor();
                process.Process(ConvertPStoPDFArgs(psFileName, pdfFileName));
                process.Dispose();
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }
        }

        #endregion

        /// CreateProcessor
        #region CreateProcessor

        /// <summary>
        /// Creates ghostscript processor
        /// </summary>
        /// <returns>ghostscript processor</returns>
        private static GhostscriptProcessor CreateProcessor()
        {
            GhostscriptVersionInfo gsVersion = new GhostscriptVersionInfo(GetLib());

            // create processor and set constructor 'fromMemory' option to true so the app will be able to run multiple instances of the Ghostscript
            GhostscriptProcessor processor = new GhostscriptProcessor(gsVersion, true);

            return processor;
        }

        #endregion
    }
}
