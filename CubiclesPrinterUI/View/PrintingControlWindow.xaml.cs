using Cubicles.API.JsonClasses;
using CubiclesPrinterLib.Data;
using CubiclesPrinterUI.Model;
using CubiclesPrinterUI.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace CubiclesPrinterUI.View
{
    /// <summary>
    /// PrintingControlWindow class.
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
        /// Constructor. Virtual printer mode.
        /// </summary>
        /// <param name="psFileName">postscript file name</param>
        /// <param name="title">print job title</param>
        public PrintingControlWindow(string psFileName, PrintJobTitle title)
            : this()
        {
            // create view model
            _viewModel = new PrintingControlViewModel(this, new PrintingControlModel(psFileName, title));
        }

        /// <summary>
        /// Constructor. Real printer mode.
        /// </summary>
        /// <param name="title">print job title</param>
        public PrintingControlWindow(PrintJobData title)
            : this()
        {
            // create view model
            _viewModel = new PrintingControlViewModel(this, new PrintingControlModel(title));
        }

        /// <summary>
        /// Constructor. Demo version.
        /// </summary>
        /// <param name="UserPC">User PC</param>
        /// <param name="DocumentName">Document Name</param>
        /// <param name="PageCount">Page Count</param>
        /// <param name="ColorPageCost">Color Page Cost</param>
        /// <param name="BlackAndWhitePageCost">Black And White Page Cost</param>
        /// <param name="isColorDocument">is color document flag</param>
        /// <param name="printers">list of printers</param>
        public PrintingControlWindow(string UserPC, string DocumentName, int PageCount, int ColorPageCost, int BlackAndWhitePageCost, bool isColorDocument, Printers printers)
            : this()
        {
            // create view model
            _viewModel = new PrintingControlViewModel(this, new PrintingControlModel(UserPC, DocumentName, PageCount, ColorPageCost, BlackAndWhitePageCost, isColorDocument, printers));
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

        /// <summary>
        /// Updates cost (shows animation) and selects recommended printer.
        /// </summary>
        /// <param name="playAnimation">play animation flag</param>
        private void UpdateCost(bool playAnimation = true)
        {
            if (_viewModel == null)
                return;

            // select recommended printer (by color)
            _viewModel.SelectRecommendedPrinter();

            if (playAnimation)
            {
                // get storyboard
                var storyboard = (Storyboard)this.FindResource("PriceChange");
                if (storyboard != null)
                    BeginStoryboard(storyboard);
            }
        }

        #endregion

        /// Event Handlers
        #region Event Handlers

        /// <summary>
        /// 'Advanced Settings' button clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void AdvancedSettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.ShowAdvancedSettings();
        }

        /// <summary>
        /// 'Print' button clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.Print();
        }

        /// <summary>
        /// Window loaded.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = _viewModel;

            UpdateCost(false);
        }

        /// <summary>
        /// 'Cancel' button clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Window unloaded.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void WindowRoot_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.Dispose();
        }

        /// <summary>
        /// Window closing.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">CancelEventArgs</param>
        private void WindowRoot_Closing(object sender, CancelEventArgs e)
        {
            if (_viewModel != null)
                _viewModel.CheckClosing();
        }

        /// <summary>
        /// 'Color' toggle clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RoutedEventArgs</param>
        private void ColorToggle_Click(object sender, RoutedEventArgs e)
        {
            UpdateCost(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }

        #endregion
    }
}