
namespace Time_Tracker.Resources.Classes
{
    class Settings
    {
        private JSONParser.JSONParser oJSONParser = new JSONParser.JSONParser();
        private string sUserSettingsFilePath = "";

        //Setting Variables
        public bool bWindowMovable = false;        

        public Settings(string sSettingsFilePath)
        {
            this.sUserSettingsFilePath = sSettingsFilePath + "\\UserSettings.json";
            LoadSettings();
        }

        private void LoadSettings()
        {
            oJSONParser.Load(sUserSettingsFilePath);

            bWindowMovable = (bool)oJSONParser.GetValue("WindowMovable", false);            
        }

        public void SaveSettings()
        {
            oJSONParser.Save(sUserSettingsFilePath);
        }
    }
}
