using System;
using System.Windows;
using System.Windows.Controls;

namespace CubiclesPrinterUI.Controls
{
    /// <summary>
    /// An extended Button class providing support for configurable Icon
    /// </summary>
    public class IconButton : Button
    {
        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string),
            typeof(IconButton), new PropertyMetadata(""));

        public String IconPath
        {
            get { return (String)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }
    }
}