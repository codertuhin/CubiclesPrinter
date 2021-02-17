using System;
using System.IO;
using Cubicles.Utility;

namespace CubiclesPrinterLib.Data
{
    /// <summary>
    /// This class contains PostScript file data
    /// </summary>
    public class PostScriptMetaData
    {
        /// Properties
        #region Properties

        /// <summary> Name of the page size (letter, a4, etc) </summary>
        public string PageSizeName { get; set; }

        /// <summary> Resolution </summary>
        public string Resolution { get; set; }

        /// <summary> Document name </summary>
        public string Title { get; set; }

        /// <summary> Document orientation </summary>
        public string Orientation { get; set; }

        /// <summary> Number of pages </summary>
        public int NumberOfPages { get; set; }

        /// <summary> Has color </summary>
        public bool? HasColor { get; set; }

        #endregion

        /// ReadFile
        #region ReadFile

        /// <summary>
        /// Reads postscript file and extracts file data
        /// </summary>
        /// <param name="fileName">file to be inspected</param>
        /// <returns>postscript file data</returns>
        public static PostScriptMetaData ReadFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            try
            {
                if (!File.Exists(fileName))
                    return null;

                // extract all the text
                string text = File.ReadAllText(fileName);
                if (string.IsNullOrWhiteSpace(text))
                    return null;

                // get lines of text
                string[] textss = text.Split(new string[] {Environment.NewLine, "\n"}, StringSplitOptions.RemoveEmptyEntries);
                if (textss == null || textss.Length < 1)
                    return null;

                // create meta data
                PostScriptMetaData data = new PostScriptMetaData();

                // fill meta data
                foreach (string line in textss)
                {
                    if (line.Contains(@"%Title:")) // Title
                    {
                        if (string.IsNullOrWhiteSpace(data.Title))
                            data.Title = GetLineValue(line);
                    }
                    else if (line.Contains(@"@PJL JOB NAME")) // Title
                        data.Title = GetLineValue2(line);
                    else if (line.Contains(@"%Orientation:")) // Orientation
                        data.Orientation = GetLineValue(line);
                    else if (line.Contains(@"%BeginFeature: *PageSize")) // PageSize
                        data.PageSizeName = GetLineValue(line);
                    else if (line.Contains(@"%BeginFeature: *Resolution")) // Resolution
                        data.Resolution = GetLineValue(line);
                    else if (line.Contains(@"%Pages:")) // Number of pages
                    {
                        int count;
                        if (int.TryParse(GetLineValue(line), out count))
                            data.NumberOfPages = count;
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets value from the postscript file line
        /// </summary>
        /// <param name="line">postscript file line</param>
        /// <returns>value from the postscript file line</returns>
        private static string GetLineValue2(string line)
        {
            try
            {
                int index = line.IndexOf('=');
                if (index > 0)
                {
                    int index2 = line.IndexOf('"', index + 1) + 1;
                    if (index2 > index)
                    {
                        int index3 = line.IndexOf('"', index2);
                        if (index3 > index2)
                        {
                            // extract value
                            return line.Substring(index2, index3 - index2).Trim();
                        }
                    }
                    else
                        // extract simple value
                        return line.Substring(index + 1).Trim();
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }

            return null;
        }


        /// <summary>
        /// Gets value from the postscript file line
        /// </summary>
        /// <param name="line">postscript file line</param>
        /// <returns>value from the postscript file line</returns>
        private static string GetLineValue(string line)
        {
            try
            {
                // find semicolon
                int index = line.IndexOf(':');
                if (index > 0)
                {
                    // find star
                    int index2 = line.LastIndexOf('*');
                    if (index2 > index)
                    {
                        // find first space after star
                        int index3 = line.IndexOf(' ', index2);
                        if (index3 > index2)
                        {
                            // extract starred value
                            return line.Substring(index3).Trim();
                        }
                    }
                    else
                        // extract simple value
                        return line.Substring(index + 1).Trim();
                }
            }
            catch (Exception ex)
            {
                WPFNotifier.DebugError(ex);
            }

            return null;
        }

        #endregion
    }
}
