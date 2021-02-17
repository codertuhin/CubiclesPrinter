using Cubicles.API.JsonClasses;
using Cubicles.Utility;
using Cubicles.Utility.Helpers;
using CubiclesPrinterUI.Controller;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CubiclesPrinterUI.Converters
{
    /// <summary>
    /// A binding converter to show correct footprints on a map
    /// </summary>
    public sealed class FootprintsConverter : IMultiValueConverter
    {
        /// Private Variables
        #region Private Variables

        /// <summary> </summary>
        private string _pathTemplate = "pack://application:,,,/CubiclesPrinterUI;component/Assets/PrinterMap/{0}-{1}.png";

        #endregion

        /// Convert
        #region Convert

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Printer destinationPrinter = values[1] as Printer;
            if (destinationPrinter == null || string.IsNullOrWhiteSpace(destinationPrinter.Name))
                return null;

            string source = values[0] as string;
            if (string.IsNullOrWhiteSpace(source))
                return null;

            // oleg : test entry for my system
            if (source.ToLower().Equals("w10"))
                source = "frontdesk1-pc";

            string internalName = EnvironmentDataController.Instance.Printers.GetValidDestination(destinationPrinter.Name, source);
            if (!string.IsNullOrWhiteSpace(internalName))
            {            
                if (source.ToLower().StartsWith("conference"))
                    source = "conference";             
                
                var path = new Uri(string.Format(_pathTemplate, source.ToLower(), internalName.ToLower()));

                try
                {
                    BitmapImage image = new BitmapImage();

                    image.BeginInit();
                    image.UriSource = path;
                    image.EndInit();

                    return image;
                }
                catch(Exception ex)
                {
                    LogHelper.LogDebug(ex);
                }                
            }

            return null;
        }

        #endregion

        /// ConvertBack
        #region ConvertBack

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}
