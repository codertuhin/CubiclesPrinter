using Cubicles.API.JsonClasses;
using CubiclesPrinterUI;
using CubiclesPrinterUI.Controller;
using CubiclesPrinterUI.View;
using System.Windows;

namespace CubiclesPrinterUIDemo
{
    public partial class MainWindow : Window
    {
        private Window activeWindow;

        public MainWindow()
        {
            InitializeComponent();

            EnvironmentDataController.Instance.Init();
        }

        private void KillActiveWindow()
        {
            if (activeWindow != null)
            {
                activeWindow.Close();
                activeWindow = null;
            }
        }

        private void Print(bool isColorDocument)
        {
            var availablePrinters = new Printers
            {
                new Printer { Name = "ColorPrinter", IsColored = true },
                new Printer { Name = "Black-WhitePrinter", IsColored = false },
                new Printer { Name = "BackPrinter", IsColored = false },
                new Printer { Name = "cubicle01_printer", IsColored = false },
                new Printer { Name = "cubicle02_printer", IsColored = false },
                new Printer { Name = "cubicle05_printer", IsColored = false }
            };

            this.KillActiveWindow();            

            activeWindow = new PrintingControlWindow(ComputerText.Text, "MySpreadsheet.xls", 22, 4, 2, isColorDocument, availablePrinters);
            activeWindow.Show();
        }

        private void PrintColorBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Print(true);
        }

        private void PrintBlackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Print(false);
        }

        private void PreparingBtn_Click(object sender, RoutedEventArgs e)
        {
            this.KillActiveWindow();

            activeWindow = new ProgressWindow
            {
                Title = "Preparing",
                Message = "Your document is being prepared for printing, please wait..."
            };

            activeWindow.Show();
        }

        private void SendingBtn_Click(object sender, RoutedEventArgs e)
        {
            this.KillActiveWindow();

            activeWindow = new ProgressWindow
            {
                Title = "Sending",
                Message = "Your document is being sent to the printer. \r\n\r\nPlease make sure the document has been fully printed before you log out."
            };

            activeWindow.Show();
        }
    }
}
