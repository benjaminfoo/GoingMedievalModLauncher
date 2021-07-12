using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;
using NSEipix.Base;

namespace GoingMedievalModLauncher
{
    // a really simple custom logger (class which writes text into files)
    public static class LoggingManager
    {
        
        private static readonly string LOG_PATH = @".\";
        private static readonly string LOG_FILE = "mod_launcher.log";
        private static readonly MemoryTarget LogMemory = new MemoryTarget();
        private static readonly MethodCallTarget LogMethod = new MethodCallTarget(
            "FlushLogger", (info, prams) =>
            {
                if ( OnFlushing != null )
                {
                    OnFlushing(info.FormattedMessage);
                }
            });

        public static event Action<string> OnFlushing;

        static LoggingManager()
        {
            // delete the previously written file when launching the logger
            var stream = File.Create(Path.Combine(LOG_PATH, LOG_FILE));
            stream.Close();

            var conf = new LoggingConfiguration();
            var logfile = new FileTarget() { FileName = LOG_FILE };
            conf.AddRuleForAllLevels(logfile);
            conf.AddRuleForAllLevels(LogMemory);
            conf.AddRuleForAllLevels(LogMethod);

            LogManager.Configuration = conf;

        }

        public static Logger getLogger<T>()
        {
            return LogManager.GetLogger(typeof(T).Name);
        }
        
        public static string GetCurrentLogs()
        {
            return string.Join("\n", LogMemory.Logs);
        }

    }
}