using System;
using System.IO;
using NSMedieval.StatsSystem;

namespace GoingMedievalModLauncher
{
    // a really simple custom logger (class which writes text into files)
    public class Logger
    {
        private static string LOG_TAG = "ModLauncher";
        private static string LOG_PATH = @".\";
        private static string LOG_FILE = "mod_launcher.log";

        // we're using a singleton here
        private static Logger INSTANCE = null;
        
        private Logger()
        {
            // delete the previously written file when launching the logger
            File.Delete(Path.Combine(LOG_PATH, LOG_FILE));
        }

        public static Logger getInstance()
        {
            if (INSTANCE == null)
            {
                INSTANCE = new Logger();
            }

            return INSTANCE;
        }

        public void info(string message)
        {
            File.AppendAllText(Path.Combine(LOG_PATH, LOG_FILE), LOG_TAG + ":"+ "\t" + message + Environment.NewLine);
        }

    }
}