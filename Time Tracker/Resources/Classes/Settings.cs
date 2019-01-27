
namespace Time_Tracker.Resources.Classes
{
    class Settings
    {
        private JSONParser.JSONParser oJSONParser = new JSONParser.JSONParser();
        private string sUserSettingsFilePath = "";

        //Setting Variables
        public bool bWindowMovable = false;        
        public bool bStartWithWindows = false;
        public bool bHideWindowAutomatically = true;
        public bool bAutomaticallyStartNewTimer = true;
        public bool bAutomaticallyStartTimerOnStart = false;
        public bool bCalculateOvertime = true;
        public bool bOvertimeCurrentMonthOnly = false;
        public bool bCalculateTimeDeficit = false;
        public int iStandardWorkTimeSeconds = 28800;

        public Settings(string sSettingsFilePath)
        {
            this.sUserSettingsFilePath = sSettingsFilePath + "\\UserSettings.json";
            LoadSettings();
        }

        private void LoadSettings()
        {
            oJSONParser.Load(sUserSettingsFilePath);

            bWindowMovable = (bool)oJSONParser.GetValue("WindowMovable", false);            
            bStartWithWindows = (bool)oJSONParser.GetValue("StartWithWindows", false);
        }

        public void SaveSettings()
        {
            oJSONParser.Save(sUserSettingsFilePath);
        }
    }
}
