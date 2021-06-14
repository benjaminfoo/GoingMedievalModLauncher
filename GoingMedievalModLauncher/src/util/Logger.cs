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
        private readonly TextWriter writer;
        private int counter = 0;

        private Logger()
        {
            // delete the previously written file when launching the logger
            File.Delete(Path.Combine(LOG_PATH, LOG_FILE));
            writer = File.AppendText(Path.Combine(LOG_PATH, LOG_FILE));
        }

        public void info(string message)
        {
            counter++;
            writer.Write(LOG_TAG + ":" + "\t" + message + Environment.NewLine);
            if ( counter >= 10 )
            {
                writer.Flush();
            }
        }

        ~Logger()
        {
            writer.Flush();
            writer.Close();
        }


    }
}