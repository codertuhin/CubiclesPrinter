using System.Windows;
using System.Windows.Controls;

namespace CubiclesPrinterUI
{
    public partial class PrintMap : UserControl
    {
        public PrintMap()
        {
            InitializeComponent();

            VisualStateManager.GoToState(this, "Glow", false);
        }
    }
}