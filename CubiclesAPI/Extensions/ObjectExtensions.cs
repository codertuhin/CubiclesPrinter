using Cubicles.API.JsonClasses;
using Cubicles.Utility.Helpers;

namespace Cubicles.Extensions
{
    /// <summary>
    /// This class contains object extension methods
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <param name="data">object to be processed</param>
        /// <returns>a string</returns>
        public static string MegaToString(this object data)
        {
            string sss = ReflectionHelper.ToString(data);
            if (string.IsNullOrWhiteSpace(sss))
                sss = ReflectionHelper.FieldsToString(data);

            return sss;
        }

        /// <summary>
        /// Converts PrinterTrouble data to string
        /// </summary>
        /// <param name="issue">printer issue</param>
        /// <returns>issue string</returns>
        public static string Issue(this PrinterTrouble printerTrouble)
        {
            string issue = "";

            if (printerTrouble == PrinterTrouble.None)
                return issue;

            if ((printerTrouble & PrinterTrouble.OutOfPaper) == PrinterTrouble.OutOfPaper)
                issue += ", Out of Paper";

            if ((printerTrouble & PrinterTrouble.OutOfToner) == PrinterTrouble.OutOfToner)
                issue += ", Out of Toner";

            if ((printerTrouble & PrinterTrouble.Offline) == PrinterTrouble.Offline)
                issue += ", Offline";

            if ((printerTrouble & PrinterTrouble.PaperJammed) == PrinterTrouble.PaperJammed)
                issue += ", Paper jammed";

            if ((printerTrouble & PrinterTrouble.Error) == PrinterTrouble.Error)
                issue += ", Error";

            if (string.IsNullOrWhiteSpace(issue))
                return null;

            if (issue[0] == ',')
                issue = issue.Substring(1).Trim();

            return issue;
        }
    }
}
