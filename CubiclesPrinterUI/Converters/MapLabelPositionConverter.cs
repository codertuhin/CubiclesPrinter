using Cubicles.API.JsonClasses;
using Cubicles.Extensions;
using Cubicles.Utility.Helpers;
using CubiclesPrinterUI.Controller;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CubiclesPrinterUI.Converters
{
    /// <summary>
    /// MapLabelPositionConverter class.
    /// </summary>
    public sealed class MapLabelPositionConverter : IValueConverter
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
            string scenario = parameter as string;
            string computerName = value as string;
            Printer printer = value as Printer;            

            // Unrecognized
            Thickness margin = new Thickness(-400, -400, 0, 0);

            // check pc name
            if (!string.IsNullOrWhiteSpace(computerName))
            {
                // oleg : test entry for my system
                if (computerName.ToLower().Equals("w10"))
                    computerName = "frontdesk1-pc";

                if (scenario.EqualsSimplified("CubicleLabel"))
                {
                    if (computerName.ToLower().StartsWith("conference"))
                        computerName = "conference";

                    margin = EnvironmentDataController.Instance.UserPCs.GetThickness(computerName);
                }
            }
            else
            {
                // check printer
                if (printer != null && !string.IsNullOrWhiteSpace(printer.Name))
                {
                    if (scenario.EqualsSimplified("PrinterLabel"))
                        margin = EnvironmentDataController.Instance.Printers.GetLabel(printer.Name);
                    else
                    if (scenario.EqualsSimplified("PrinterGlow"))
                        margin = EnvironmentDataController.Instance.Printers.GetGlow(printer.Name);
                }
            }

            if (Environment.MachineName.EqualsSimplified("W10"))
                margin = new Thickness(25);

            /*
            if (printer != null)
                LogHelper.LogDebug(computerName + " : " + printer.Name + "[" + scenario + "] : " + margin.ToString());
            else
                LogHelper.LogDebug(computerName + "[" + scenario + "] : " + margin.ToString());*/

            return margin;
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