using System;

namespace Cubicles.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// ToBool
        #region ToBool

        /// <summary>
        /// Converts String to Bool.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Bool datatype of the string after conversion. False if conversion failed.</returns>
        public static bool ToBool(this String value)
        {
            return ToBool(value, false);
        }
        /// <summary>
        /// Converts String to Bool.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <param name="trueIfHasValue">Boolean flag to set the result to true if the input object has value regardless of the content.</param>
        /// <returns>Bool datatype of the string after conversion. False if conversion failed.</returns>
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
        /// Extension method for String DataType.
        /// Converts String to Int.
        /// </summary>
        /// <param name="value">String to convert.</param>
        /// <returns>Int datatype of the string after conversion.Zero(0) if conversion failed.</returns>
        public static int ToInt(this String value)
        {
            int retVal = 0;
            if (!string.IsNullOrEmpty(value))
                int.TryParse(value, out retVal);
            return retVal;
        }
        
        #endregion
    }
}
