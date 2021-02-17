using System;
using System.Globalization;
using System.Windows.Data;

namespace CubiclesPrinterUI.Converters
{
    /// <summary>
    /// RadioButtonCheckedConverter class
    /// </summary>
    public class RadioButtonCheckedConverter : IValueConverter
    {
        /// Convert
        #region Convert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        #endregion

        /// ConvertBack
        #region ConvertBack

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }

        #endregion
    }
}