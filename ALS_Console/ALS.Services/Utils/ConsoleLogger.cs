using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;

namespace ALS.Services.Utils
{
    public static class ConsoleLogger
    {
        private static readonly int _maxLevelLength;

        static ConsoleLogger()
        {
            _maxLevelLength = Enum.GetNames(typeof(LogLevel)).Max(q => q.Length);
        }

        public static void LogCritical(string log)
        {
            Log(LogLevel.Critical, log);
        }

        public static void LogDebug(string log)
        {
            Log(LogLevel.Debug, log);
        }

        public static void LogError(string log)
        {
            Log(LogLevel.Error, log);
        }

        public static void LogInformation(string log)
        {
            Log(LogLevel.Information, log);
        }

        public static void LogTrace(string log)
        {
            Log(LogLevel.Trace, log);
        }

        public static void LogWarning(string log)
        {
            Log(LogLevel.Warning, log);
        }

        public static void Log(LogLevel level, string log)
        {
            var levelStr = level.ToString().PadRight(_maxLevelLength);

            var msg = string.Format("{0} {1} {2} ", DateTime.Now.ToLocalTime(), levelStr, log);
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }

        public static void LogNewLine()
        {
            Console.WriteLine();
        }
    }
}
