using System;

namespace Cubicles.Extensions
{
    /// <summary>
    /// This class contains string extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool EqualsSimplified(this string A, string B)
        {
            if (A == null)
                return false;

            if (B == null)
                return false;

            A = A.ToLower();
            B = B.ToLower();

            return A.Equals(B);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static bool ContainsSimplified(this string A, string B)
        {
            if (A == null)
                return false;

            if (B == null)
                return false;

            A = A.ToLower();
            B = B.ToLower();

            return A.Contains(B);
        }

        /// ToBool
        #region ToBool

        /// <summary>
        /// Converts string to bool.
        /// </summary>
        /// <param name="value">string to be convert</param>
        /// <returns>bool datatype of the string after conversion. False if conversion failed.</returns>
        public static bool ToBool(this string value)
        {
            return ToBool(value, false);
        }
        /// <summary>
        /// Converts string to bool.
        /// </summary>
        /// <param name="value">String to be converted</param>
        /// <param name="trueIfHasValue">boolean flag to set the result to true if the input object has value regardless of the content.</param>
        /// <returns>bool datatype of the string after conversion. False if conversion failed.</returns>
        public static bool ToBool(this String value, bool trueIfHasValue)
        {
            bool retVal = false;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (trueIfHasValue == false)
                {
                    bool.TryParse(value, out retVal);
                    if (retVal == false && value.ToInt() == 1)
                        retVal = true;
                }
                else
                {
                    if (value.Trim() != string.Empty)
                        retVal = true;
                }
            }
            return retVal;
        }
        
        #endregion

        /// Short
        #region Short

        /// <summary>
        /// Shortens string entry by the specified max length
        /// </summary>
        /// <param name="value">string to be shortened</param>
        /// <param name="maxLen">max length of string</param>
        /// <returns>shortened string</returns>
        public static string Short(this string value, int maxLen = 66)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            if (maxLen < 1)
                return value;

            if (value.Length > maxLen)
            {
                int off = 4;
                return value.Substring(0, maxLen - off) + "~" + value.Substring(value.Length - off);
            }

            return value;
        }

        #endregion

        /// IsNumeric
        #region IsNumeric

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this String value)
        {
            int retVal;
            return int.TryParse(value, out retVal);
        }

        /// <summary>
        /// Converts string to int
        /// </summary>
        /// <param name="value">string to convert</param>
        /// <returns>int datatype of the string after conversion.Zero(0) if conversion failed.</returns>
        public static int ToInt(this string value)
        {
            int retVal = 0;
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out retVal);
            return retVal;
        }
        
        #endregion
    }
}
