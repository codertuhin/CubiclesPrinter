using Cubicles.Utility.Helpers;
using CubiclesPrinterUI.Controller;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CubiclesPrinterUI.Converters
{
    /// <summary>
    /// MapLabelVisibilityConverter class.
    /// </summary>
    public sealed class MapLabelVisibilityConverter : IValueConverter
    {   
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
            string placeName = value as string;
            string scenario = parameter as string;            

            if (!string.IsNullOrWhiteSpace(placeName) && !string.IsNullOrWhiteSpace(scenario))
            {
                // oleg : test entry for my system
                if (placeName.ToLower().Equals("w10"))
                    placeName = "frontdesk1-pc";

                if (placeName.ToLower().StartsWith("conference"))
                    placeName = "conference";   
                
                if ((scenario == "Top" && EnvironmentDataController.Instance.UserPCs.HasPC(placeName, Classes.MapPosition.Top)) ||
                    (scenario == "Side" && EnvironmentDataController.Instance.UserPCs.HasPC(placeName, Classes.MapPosition.Side)) ||
                    (scenario == "Bottom" && EnvironmentDataController.Instance.UserPCs.HasPC(placeName, Classes.MapPosition.Bottom)))
                {
                    return Visibility.Visible;
                }
            }

            LogHelper.LogDebug(placeName + " : " + scenario);
            return Visibility.Collapsed;
        }

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
            throw new NotImplementedException();
        }
    }
}