using System.Collections.Generic;
using System.Text;

namespace Cubicles.Extensions
{
    /// <summary>
    /// This class contains dictionary extension methods
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Formats dictionary to a query string
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static string GetAsQueryString(this Dictionary<string, string> dic)
        {
            if (dic == null || dic.Count < 1)
                return "";

            StringBuilder sb = new StringBuilder();
            foreach (var kvp in dic)
                sb.Append(string.Format("{0}={1}&", kvp.Key, kvp.Value));

            if (sb[sb.Length - 1] == '&')
                sb.Length = sb.Length - 1;

            return sb.ToString();
        }
    }
}
