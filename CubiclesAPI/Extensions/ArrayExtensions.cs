using System;
using System.Text;

namespace Cubicles.Extensions
{
    /// <summary>
    /// This class contains array extension methods
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Converts array to csv line
        /// </summary>
        /// <param name="array">array to be converted</param>
        /// <returns>csv line</returns>
        public static string ToCSV(this Array array)
        {
            return ToStringWithSeparator(array, ", ");
        }

        /// <summary>
        /// Converts array to string line with the specified separator
        /// </summary>
        /// <param name="array">array to be converted</param>
        /// <param name="separator">separator</param>
        /// <returns>string representation</returns>
        public static string ToStringWithSeparator(this Array array, string separator = ", ")
        {
            if (array == null || array.Length < 1)
                return null;

            if (string.IsNullOrWhiteSpace(separator))
                separator = ", ";

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.Length - 1; i++)
                sb.Append(array.GetValue(i) + separator);

            sb.Append(array.GetValue(array.Length - 1));

            return sb.ToString();
        }
    }
}
