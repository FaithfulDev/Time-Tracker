using System;
using System.Windows;

namespace Time_Tracker
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetStartupPosition();
        }

        private void SetStartupPosition()
        {
            double iScreenHeight = SystemParameters.WorkArea.Height;
            double iScreenWidth = SystemParameters.WorkArea.Width;

            this.Left = iScreenWidth - this.Width - 10;
            this.Top = iScreenHeight - this.Height - 10;
        }

        private void UiTrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Visible;
            this.Activate();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }

        private void MenuItem_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
