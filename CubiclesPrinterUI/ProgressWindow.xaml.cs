using System.Windows;

namespace CubiclesPrinterUI
{
    public partial class ProgressWindow : Window
    {
        // Dependency Properties
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string),
            typeof(ProgressWindow), new PropertyMetadata(null));

        public ProgressWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        // Public Properties
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
                DragMove();
        }
    }
}