using System;
using System.IO;
using NSEipix.Base;

namespace GoingMedievalModLauncher
{
    // a really simple custom logger (class which writes text into files)
    public class Logger : Singleton<Logger>
    {

        private static string LOG_TAG = "ModLauncher";
        private static string LOG_PATH = @".\";
        private static string LOG_FILE = "mod_launcher.log";
        private object l = new object();

        public event Action<string> OnFlushing; 

        private Logger()
        {
            // delete the previously written file when launching the logger
            var stream = File.Create(Path.Combine(LOG_PATH, LOG_FILE));
            stream.Close();
        }

        public void info(string message)
        {
            lock (l)
            {
                var a = File.AppendText(Path.Combine(LOG_PATH, LOG_FILE));
                a.Write(LOG_TAG + ":" + "\t" + message + Environment.NewLine);
                a.Close();
                if(OnFlushing != null)
                    OnFlushing(LOG_TAG + ":" + "\t" + message + Environment.NewLine);
            }
        }

        public string GetCurrentLogs()
        {
            lock (l)
            {
                return File.ReadAllText(Path.Combine(LOG_PATH, LOG_FILE));
            }
        }

    }
}