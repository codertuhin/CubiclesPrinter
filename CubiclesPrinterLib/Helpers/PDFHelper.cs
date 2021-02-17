using System;
using System.Drawing.Printing;
using System.IO;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using PdfiumViewer;

namespace CubiclesPrinterLib.Helpers
{
    /// <summary>
    /// This class contains methods for work with pdf
    /// </summary>
    public class PDFHelper
    {
        /// Print
        #region Print
        
        /// <summary>
        /// Prints PDF
        /// </summary>
        /// <param name="printer">printer settings</param>
        /// <param name="fileName">file name</param>
        /// <param name="printerName">printer name</param>
        /// <param name="documentName">document name</param>
        /// <returns></returns>
        public static bool Print_Default(PrinterSettings printer, string fileName, string documentName, string owner, string host)
        {
            try
            {
                if (!File.Exists(fileName))
                {
                    LogHelper.Log("No file found : " + fileName);
                    return false;
                }

                // Now print the PDF document
                using (var document = PdfDocument.Load(fileName))
                {
                    using (var printDocument = document.CreatePrintDocument())
                    {
                        printDocument.DocumentName = documentName;
                        printDocument.PrinterSettings = printer;
                        printDocument.DefaultPageSettings = printer.DefaultPageSettings;
                        printDocument.PrintController = new StandardPrintController();
                        printDocument.Print();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                WPFNotifier.Error(ex);
                return false;
            }
        }

        public static bool Print(PrinterSettings printer, string fileName, string documentName, string owner, string host)
        {
            return Print_Default(printer, fileName, documentName, owner, host);
        }

        #endregion

        /// IsPDF
        #region IsPDF

        /// <summary>
        /// Determines whether it's PDF (supported) or not
        /// </summary>
        /// <param name="fileName">file to be inspected</param>
        /// <returns>true if PDF; otherwise false</returns>
        public static bool IsPDF(string fileName)
        {
            try
            {
                // try to load PDF
                using (var document = PdfDocument.Load(fileName))
                {
                    // if successful then it's supported PDF
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        #endregion

        public static bool HasColor(string fileName)
        {
            LogHelper.LogDebug(fileName);

            return GhostScriptHelper.HasColor(fileName);
        }
    }
}
