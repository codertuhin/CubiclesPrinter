using System;
using System.Collections.Generic;
using System.Management;
using Cubicles.Utility.Helpers;
using CubiclesPrinterLib.Helpers;

namespace CubiclesPrinterLib.Data
{
    /// <summary>
    /// This class represents print job title
    /// </summary>
    public class PrintJobTitle
    {
        /// <summary> Printer name </summary>
        public string PrinterName { get; set; }

        /// <summary> Owner </summary>
        public string Owner { get; set; }

        /// <summary> Host </summary>
        public string Host { get; set; }

        /// <summary> Printer document name </summary>
        public string Document { get; set; }

        /// <summary> Total pages count </summary>
        public int TotalPages { get; set; }

        /// <summary> Total pages count </summary>
        public int JobID { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public PrintJobTitle()
        {
        }

        public PrintJobTitle(ManagementObject printJob)
        {
            try
            {
                string prnterName;
                int prntJobID;
                PrintHelper.ExtractNameAndId(printJob, out prnterName, out prntJobID);

                PrinterName = prnterName;
                JobID = prntJobID;
                Document = PrintHelper.ExtractDocumentName(printJob);
                Host = PrintHelper.ExtractHost(printJob);
                Owner = PrintHelper.ExtractOwner(printJob);
                if (!string.IsNullOrWhiteSpace(Host))
                    Host = Host.Replace("\\", "").Trim();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
            
        }

        public PrintJobTitle(ManagementBaseObject printJob)
        {
            try
            {
                string prnterName;
                int prntJobID;
                PrintHelper.ExtractNameAndId(printJob, out prnterName, out prntJobID);

                PrinterName = prnterName;
                JobID = prntJobID;
                Document = PrintHelper.ExtractDocumentName(printJob);
                Host = PrintHelper.ExtractHost(printJob);
                Owner = PrintHelper.ExtractOwner(printJob);
                if (!string.IsNullOrWhiteSpace(Host))
                    Host = Host.Replace("\\", "").Trim();
            }
            catch (Exception ex)
            {
                LogHelper.Log(ex);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="document">document name</param>
        public PrintJobTitle(string printerName, string document, string owner, string host, int jobID)
        {
            PrinterName = printerName;
            Document = document;
            Owner = owner;
            Host = host;
            JobID = jobID;
        }

        /// Overrides
        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ReflectionHelper.ToString(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PrintJobTitle title = obj as PrintJobTitle;
            if (title == null)
                return false;

            LogHelper.LogDebug(this + " & " + title);

            if (title.PrinterName == PrinterName && title.Document == Document && title.Owner == Owner && title.Host == Host && title.JobID == JobID)
                return true;

            LogHelper.LogDebug("Not equal");

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        #endregion
    }

    /// <summary>
    /// This class represents print job title list
    /// </summary>
    public class PrintJobTitles : List<PrintJobTitle>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public void Add(PrintJobData data)
        {
            if (data == null)
                return;

            Add(data.PrintJobTitle);
        }


        /// <summary>
        /// Removes specified title from the list
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="documentName">document name</param>
        public void RemoveTitle(string printerName, string documentName, string owner, string host, int jobID)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return;

            if (string.IsNullOrWhiteSpace(documentName))
                return;

            if (string.IsNullOrWhiteSpace(owner))
                return;

            for(int i = Count - 1; i >= 0; i--)
            {
                if (this[i].PrinterName == printerName && this[i].Document == documentName && this[i].Owner == owner && this[i].Host == host)
                {
                    if (this[i].JobID == jobID || this[i].JobID == -1)
                        RemoveAt(i);
                }
            }
        }

        public void RemoveTitle(PrintJobData data)
        {
            if (data == null)
                return;

            RemoveTitle(data.PrintJobTitle);
        }

        public void RemoveTitle(PrintJobTitle title)
        {
            if (title == null)
                return;

            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i].Equals(title))
                {
                    RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Indicates whether list contains specified job title
        /// </summary>
        /// <param name="data">print job data</param>
        /// <returns></returns>
        public bool HasTitle(PrintJobData data)
        {
            if (data == null)
                return false;

            if (data.PrintJobTitle == null)
                return false;

            LogHelper.LogDebug(data.PrintJobTitle.ToString());
            return HasTitle(data.PrintJobTitle.PrinterName, data.PrintJobTitle.Document, data.PrintJobTitle.Owner, data.PrintJobTitle.Host, data.PrintJobTitle.JobID);
        }

        public bool HasTitleAndMorePages(PrintJobData data)
        {
            if (data == null)
                return false;

            if (data.PrintJobTitle == null)
                return false;

            return HasTitleAndMorePages(data.PrintJobTitle.PrinterName, data.PrintJobTitle.Document, data.PrintJobTitle.Owner, data.PrintJobTitle.Host, data.PrintJobTitle.TotalPages);
        }

        public bool HasTitleAndMorePages(string printerName, string documentName, string owner, string host, int pages)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            if (string.IsNullOrWhiteSpace(documentName))
                return false;

            if (string.IsNullOrWhiteSpace(owner))
                return false;

            if (string.IsNullOrWhiteSpace(host))
                return false;

            foreach (var item in this)
            {
                if (item.PrinterName == printerName && item.Document == documentName && item.Owner == owner && item.Host == host)
                {
                    if (item.TotalPages > pages)
                    {
                        item.TotalPages = pages;
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether list contains specified job title
        /// </summary>
        /// <param name="printerName">printer name</param>
        /// <param name="documentName">document name</param>
        /// <returns></returns>
        public bool HasTitle(string printerName, string documentName, string owner, string host, int jobID)
        {
            if (string.IsNullOrWhiteSpace(printerName))
                return false;

            if (string.IsNullOrWhiteSpace(documentName))
                return false;

            if (string.IsNullOrWhiteSpace(owner))
                return false;

            if (string.IsNullOrWhiteSpace(host))
                return false;

            LogHelper.LogDebug(printerName);
            foreach (var item in this)
            {
                LogHelper.LogDebug(item.ToString());
                if (item.PrinterName == printerName && item.Document == documentName && item.Owner == owner && item.Host == host)
                {
                    if (item.JobID == jobID)
                        return true;

                    if (item.JobID == -1)
                    {
                        item.JobID = jobID;
                        return true;
                    }

                    if (jobID == -1)
                        return true;
                }
            }

            return false;
        }
    }
}
