using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CubiclesPrinterUI.Converters
{
    /// <summary>
    /// BooleanToVisibilityConverter class.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// Convert
        #region Convert

        /// <summary>
        /// Converts boolean value to visibility.
        /// </summary>
        /// <param name="value">boolean</param>
        /// <param name="targetType">targetType</param>
        /// <param name="parameter">parameter</param>
        /// <param name="culture">culture</param>
        /// <returns>visibility</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        #endregion

        /// ConvertBack
        #region ConvertBack

        /// <summary>
        /// Converts visibility value to boolean.
        /// </summary>
        /// <param name="value">visibility</param>
        /// <param name="targetType">targetType</param>
        /// <param name="parameter">parameter</param>
        /// <param name="culture">culture</param>
        /// <returns>boolean</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility Value && Value == Visibility.Visible)
                return true;

            return false;
        }

        #endregion
    }
}