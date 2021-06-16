using System;
using System.IO;
using NSEipix.Base;
using NSMedieval.StatsSystem;

namespace GoingMedievalModLauncher
{
    // a really simple custom logger (class which writes text into files)
    public class Logger : Singleton<Logger>
    {
        private static string LOG_TAG = "ModLauncher";
        private static string LOG_PATH = @".\";
        private static string LOG_FILE = "mod_launcher.log";

        private Logger()
        {
            // delete the previously written file when launching the logger
            File.Delete(Path.Combine(LOG_PATH, LOG_FILE));
        }

        public void info(string message)
        {
            File.AppendAllText(Path.Combine(LOG_PATH, LOG_FILE), LOG_TAG + ":"+ "\t" + message + Environment.NewLine);
        }


    }
}