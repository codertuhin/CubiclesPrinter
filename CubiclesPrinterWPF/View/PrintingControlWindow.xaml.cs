using CubiclesPrinterLib.Data;
using CubiclesPrinterWPF.Model;
using CubiclesPrinterWPF.ViewModel;
using System.Windows;

namespace CubiclesPrinterWPF.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PrintingControlWindow : Window
    {
        /// Private Variables
        #region Private Variables

        /// <summary> View model </summary>
        private PrintingControlViewModel _viewModel;

        #endregion

        /// Properties
        #region Properties

        /// <summary> View model for external access </summary>
        public PrintingControlViewModel ViewModel { get { return _viewModel; } }

        #endregion

        /// Init
        #region Init

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="psFileName">postscript file name</param>
        /// <param name="title">print job title</param>
        public PrintingControlWindow(string psFileName, PrintJobTitle title)
            :this()
        {
            // create view model
            _viewModel = new PrintingControlViewModel(this, new PrintingControlModel(psFileName, title));
        }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="title">print job title</param>
        public PrintingControlWindow(PrintJobData title)
            : this()
        {
            // create view model
            _viewModel = new PrintingControlViewModel(this, new PrintingControlModel(title));
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        PrintingControlWindow()
        {
            InitializeComponent();
        }

        #endregion

        /// Methods
        #region Methods

        /// <summary>
        /// Updates data with the new one.
        /// </summary>
        /// <param name="data">new data</param>
        public void UpdateData(PrintJobData data)
        {
            // update data
            _viewModel.UpdateData(data);
        }

        #endregion

        /// Event Handlers
        #region Event Handlers

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAdvancedSettings_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ShowAdvancedSettings();
        }

        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Print();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;
        }

        private void CubiclesPrintingControlMain_Closed(object sender, System.EventArgs e)
        {
            if (_viewModel != null)
                _viewModel.CheckClosing();
        }

        #endregion
    }
}
