using System;
using System.Windows;
using System.Windows.Threading;
using Time_Tracker.Resources.Classes;

namespace Time_Tracker
{
    public partial class MainWindow : Window
    {
        private Database oDatabase;
        private DateTime dStart;
        private DispatcherTimer oTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            SetStartupPosition();

            oTimer.Tick += new EventHandler(Timer_Tick);
            oTimer.Interval = new TimeSpan(0, 0, 1);            

            oDatabase = new Database(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TimeTracker");
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (oTimer.IsEnabled)
            {
                if(MessageBox.Show("If you quit the App now your current timer will be lost. Quit anyway?",
                                "Quit?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void MenuItem_Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        private void UiClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UiStart_Click(object sender, RoutedEventArgs e)
        {
            //Do nothing if timer is already running.
            if (oTimer.IsEnabled)
            {
                return;
            }

            //-1 second to create a 1 second elapsed time right away (looks weird if it starts at 0).
            dStart = DateTime.Now.AddSeconds(-1);
            this.uiActivity.Text = "";
            this.uiDescription.Text = "";

            //Call the tick event directly to make it start right away.
            Timer_Tick(null, null);
            oTimer.Start();
        }

        private void UiStop_Click(object sender, RoutedEventArgs e)
        {
            oTimer.Stop();

            DateTime dEnd = DateTime.Now;

            oDatabase.Save(new Database.TimeRecord(0, this.dStart, dEnd, this.uiActivity.Text, this.uiDescription.Text));

            //Start new timer 
            UiStart_Click(null, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.uiTime.Content = DateTime.Parse((DateTime.Now - dStart).ToString()).ToString("HH:mm:ss");
        }
    }
}
