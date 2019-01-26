using System;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;
using Time_Tracker.Resources.Classes;

namespace Time_Tracker
{
    public partial class MainWindow : Window
    {
        private string sAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TimeTracker";
        private Database oDatabase;
        private Settings oSettings;
        private DateTime dStart;
        private DispatcherTimer oTimer = new DispatcherTimer();
        private List<Database.TimeRecord> cTodaysTimes;
        private int iStandardWorkTimeSeconds = 28800;
        private int iWorkedTimeSeconds;
        private int iOverTime;

        public MainWindow()
        {
            InitializeComponent();
            SetStartupPosition();

            oTimer.Tick += new EventHandler(Timer_Tick);
            oTimer.Interval = new TimeSpan(0, 0, 1);            

            oDatabase = new Database(sAppDataPath);
            oSettings = new Settings(sAppDataPath);
            ApplySettings();

            cTodaysTimes = oDatabase.GetTodaysTimes();
            CalculateWorkedTime();

            UpdateView();
            MakeWindowUnMovable();
        }

        private void ApplySettings()
        {
            if (oSettings.bWindowMovable){ MakeWindowMovable(); }else{ MakeWindowUnMovable(); }
            SetStartWithWindows();
        }

        private void CalculateWorkedTime()
        {
            iWorkedTimeSeconds = 0;

            foreach (Database.TimeRecord oTime in cTodaysTimes)
            {
                iWorkedTimeSeconds += (int)Math.Round((oTime.dEnd - oTime.dStart).TotalSeconds, 0);
            }
            
            iOverTime = oDatabase.GetOverTime(iStandardWorkTimeSeconds);
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
            if (oSettings.bHideWindowAutomatically)
            {
                this.Visibility = Visibility.Collapsed;
            }            
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
            this.Visibility = Visibility.Collapsed;
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

            oTimer.Start();

            //Update View so that the timer gets updated right away, instead of in 1 second.
            UpdateView();            
        }

        private void UiStop_Click(object sender, RoutedEventArgs e)
        {
            oTimer.Stop();

            DateTime dEnd = DateTime.Now.AddSeconds(-1);
            
            Database.TimeRecord oTimeRecord = new Database.TimeRecord(0, this.dStart, dEnd, this.uiActivity.Text, this.uiDescription.Text);
            oDatabase.Save(oTimeRecord);
            cTodaysTimes.Add(oTimeRecord);
            
            CalculateWorkedTime();

            //Start new timer 
            UiStart_Click(null, null);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (oTimer.IsEnabled)
            {
                this.uiTime.Content = DateTime.Parse((DateTime.Now - dStart).ToString()).ToString("HH:mm:ss");
            }
            else
            {
                this.uiTime.Content = "--:--:--";
                dStart = DateTime.Now;
            }

            string sWorkedTime = "";
            string sOverTime = "";

            int iWorkedTimePlusCurrent = iWorkedTimeSeconds + (int)Math.Round((DateTime.Now - dStart).TotalSeconds, 0);

            if (iWorkedTimePlusCurrent < iStandardWorkTimeSeconds)
            {
                sWorkedTime = TimeSpan.FromSeconds(iWorkedTimePlusCurrent).ToString(@"hh\:mm\:ss");
                sOverTime = (iOverTime >= 0 ? "+" : "-")
                        + TimeSpan.FromSeconds((iOverTime >= 0 ? iOverTime : iOverTime * (-1))).ToString(@"hh\:mm");
            }
            else
            {
                int iOverTimePlusCurrent = iOverTime + (iWorkedTimePlusCurrent - iStandardWorkTimeSeconds);
                sWorkedTime = TimeSpan.FromSeconds(iStandardWorkTimeSeconds).ToString(@"hh\:mm\:ss");
                sOverTime = (iOverTimePlusCurrent >= 0 ? "+" : "-")
                        + TimeSpan.FromSeconds((iOverTimePlusCurrent >= 0 ? iOverTimePlusCurrent : iOverTimePlusCurrent * (-1))).ToString(@"hh\:mm");
            }

            this.uiOvertime.Content = $"{sWorkedTime} ({sOverTime})";
        }

        private void DragWindow_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void MakeWindowMovable()
        {
            if(this.uiDragAreaRow.Height.ToString() == "0")
            {
                this.uiDragAreaRow.Height = new GridLength(10);
                this.Height += 10;
                this.Top -= 10;
            }            
        }

        private void MakeWindowUnMovable()
        {
            if(this.uiDragAreaRow.Height.ToString() == "10")
            {
                this.uiDragAreaRow.Height = new GridLength(0);
                this.Height -= 10;
                this.Top += 10;
            } 
        }

        private void SetStartWithWindows()
        {
            var sPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            Microsoft.Win32.RegistryKey oKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(sPath, true);

            if (oSettings.bStartWithWindows)
            {
                oKey.SetValue("TimeTracker", System.Windows.Forms.Application.ExecutablePath.ToString());
            }
            else
            {
                oKey.DeleteValue("TimeTracker", false);
            }
        }
    }
}
