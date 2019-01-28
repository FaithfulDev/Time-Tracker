
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
            if (System.IO.File.Exists(sUserSettingsFilePath))
            {
                oJSONParser.Load(sUserSettingsFilePath);
            }            

            bWindowMovable = (bool)oJSONParser.GetValue("WindowMovable", false);            
            bStartWithWindows = (bool)oJSONParser.GetValue("StartWithWindows", false);
            bHideWindowAutomatically = (bool)oJSONParser.GetValue("HideWindowAutomatically", true);
            bAutomaticallyStartNewTimer = (bool)oJSONParser.GetValue("AutomaticallyStartNewTimer", true);
            bAutomaticallyStartTimerOnStart = (bool)oJSONParser.GetValue("AutomaticallyStartTimerOnStart", false);
            bCalculateOvertime = (bool)oJSONParser.GetValue("CalculateOvertime", true);
            bOvertimeCurrentMonthOnly = (bool)oJSONParser.GetValue("OvertimeCurrentMonthOnly", false);
            bCalculateTimeDeficit = (bool)oJSONParser.GetValue("CalculateTimeDeficit", false);
            iStandardWorkTimeSeconds = (int)oJSONParser.GetValue("StandardWorkTimeSeconds", 28800);
        }

        public void SaveSettings()
        {
            oJSONParser.SetValue("WindowMovable", bWindowMovable);
            oJSONParser.SetValue("StartWithWindows", bStartWithWindows);
            oJSONParser.SetValue("HideWindowAutomatically", bHideWindowAutomatically);
            oJSONParser.SetValue("AutomaticallyStartNewTimer", bAutomaticallyStartNewTimer);
            oJSONParser.SetValue("AutomaticallyStartTimerOnStart", bAutomaticallyStartTimerOnStart);
            oJSONParser.SetValue("CalculateOvertime", bCalculateOvertime);
            oJSONParser.SetValue("OvertimeCurrentMonthOnly", bOvertimeCurrentMonthOnly);
            oJSONParser.SetValue("CalculateTimeDeficit", bCalculateTimeDeficit);
            oJSONParser.SetValue("StandardWorkTimeSeconds", iStandardWorkTimeSeconds);

            oJSONParser.Save(sUserSettingsFilePath);
        }
    }
}
