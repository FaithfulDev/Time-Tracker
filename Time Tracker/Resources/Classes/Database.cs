using System;
using System.Data.SQLite;
using System.Collections.Generic;

namespace Time_Tracker.Resources.Classes
{
    class Database
    {
        private string sDatabasePath;
        
        /// <summary>
        /// Initialize a new database. Folders and database will be created if they not already exist.
        /// </summary>
        /// <param name="sDatabasePath">Path where the database is supposed to be saved.</param>
        public Database(string sDatabasePath)
        {
            this.sDatabasePath = sDatabasePath;
            System.IO.Directory.CreateDirectory(sDatabasePath);

            SQLiteConnection oSQLiteConnection = new SQLiteConnection($"Data Source={sDatabasePath}\\TimeTracker.db;Version=3;foreign keys=true;");
            oSQLiteConnection.Open();

            bool bExist = false;
            SQLiteDataReader oSQLiteReader =  new SQLiteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name='TimeTracker';",
                                                                oSQLiteConnection).ExecuteReader();
            while (oSQLiteReader.Read())
            {
                bExist = true;
                break;
            }

            if (!bExist)
            {
                new SQLiteCommand(@"CREATE TABLE TimeTracker (
                                    id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE,
                                    start DATETIME NOT NULL,
                                    [end] DATETIME NOT NULL,
                                    activity TEXT,
                                    description TEXT);", oSQLiteConnection).ExecuteNonQuery();
            }

            oSQLiteConnection.Close();
        }

        /// <summary>
        /// Save a time record.
        /// </summary>
        /// <param name="oTimeRecord"></param>
        /// <returns>True if successful</returns>
        public bool Save(TimeRecord oTimeRecord)
        {
            SQLiteConnection oSQLiteConnection = new SQLiteConnection($"Data Source={sDatabasePath}\\TimeTracker.db;Version=3;foreign keys=true;");
            oSQLiteConnection.Open();

            string sSQL = $@"INSERT INTO TimeTracker(start, [end], activity, description) VALUES 
                             ('{oTimeRecord.dStart.ToString("yyyy-MM-dd HH:mm:ss")}', 
                              '{oTimeRecord.dEnd.ToString("yyyy-MM-dd HH:mm:ss")}', 
                               {(oTimeRecord.sActivitiy != "" ? "'" + oTimeRecord.sActivitiy.Replace("'", "''") + "'":"NULL")}, 
                               {(oTimeRecord.sDescription != "" ? "'" + oTimeRecord.sDescription.Replace("'", "''") + "'" : "NULL")});";

            new SQLiteCommand(sSQL, oSQLiteConnection).ExecuteNonQuery();

            oSQLiteConnection.Close();

            return true;
        }

        public List<TimeRecord> GetTodaysTimes()
        {
            List<TimeRecord> cTodaysTimes = new List<TimeRecord>();

            SQLiteConnection oSQLiteConnection = new SQLiteConnection($"Data Source={sDatabasePath}\\TimeTracker.db;Version=3;foreign keys=true;");
            oSQLiteConnection.Open();

            string sSQL = $@"SELECT * FROM TimeTracker WHERE start >= date('now')";

            SQLiteDataReader oSQLiteReader = new SQLiteCommand(sSQL, oSQLiteConnection).ExecuteReader();

            while (oSQLiteReader.Read())
            {
                TimeRecord oTimeRecord =
                    new TimeRecord(
                        int.Parse(oSQLiteReader["id"].ToString()),
                        DateTime.Parse(oSQLiteReader["start"].ToString()),
                        DateTime.Parse(oSQLiteReader["end"].ToString()),
                        oSQLiteReader["activity"].ToString(),
                        oSQLiteReader["description"].ToString()
                    );

                cTodaysTimes.Add(oTimeRecord);
            }

            oSQLiteConnection.Close();

            return cTodaysTimes;
        }

        public int GetOverTime(int iStandardWorkTimeSeconds)
        {
            int iOverTime = 0;

            SQLiteConnection oSQLiteConnection = new SQLiteConnection($"Data Source={sDatabasePath}\\TimeTracker.db;Version=3;foreign keys=true;");
            oSQLiteConnection.Open();

            string sSQL = 
                $@"SELECT 
                     (SELECT count(DISTINCT date(start)) from TimeTracker WHERE date(start) != date('now', 'localtime')) * {iStandardWorkTimeSeconds} TotalBaseTime,
                     ifnull((SELECT sum(strftime('%s', [end]) - strftime('%s', start)) FROM TimeTracker WHERE date(start) != date('now', 'localtime')), 0) TotalActualTime
                   FROM TimeTracker LIMIT 1";

            SQLiteDataReader oSQLiteReader = new SQLiteCommand(sSQL, oSQLiteConnection).ExecuteReader();

            while (oSQLiteReader.Read())
            {                 
                iOverTime = int.Parse(oSQLiteReader["TotalActualTime"].ToString()) 
                            - int.Parse(oSQLiteReader["TotalBaseTime"].ToString());
            }

            oSQLiteConnection.Close();

            return iOverTime;
        }

        /// <summary>
        /// Class for storing time record data.
        /// </summary>
        public class TimeRecord
        {
            public int iID;
            public DateTime dStart;
            public DateTime dEnd;
            public string sActivitiy;
            public string sDescription;

            public TimeRecord(int iID, DateTime dStart, DateTime dEnd, string sActivitiy, string sDescription)
            {
                this.iID = iID;
                this.dStart = dStart;
                this.dEnd = dEnd;
                this.sActivitiy = sActivitiy;
                this.sDescription = sDescription;
            }
        }        
    }
}
