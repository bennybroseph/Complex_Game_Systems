namespace Utility
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Debug
    {
        private static Stopwatch s_Stopwatch;

        public static void Init()
        {
            s_Stopwatch = Stopwatch.StartNew();
            WriteToLog("Debug Initialized");
        }

        public static void Log(
            object obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            Console.WriteLine(obj);

            WriteToLog(obj, lineNumber, caller, filePath);
        }

        public static void LogWarning(
            object obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            obj += "Warning";

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Log(obj, lineNumber, caller, filePath);

            Console.ForegroundColor = oldColor;
        }

        public static void LogError(
            object obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            obj += "Error: ";

            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Log(obj, lineNumber, caller, filePath);

            Console.ForegroundColor = oldColor;
        }

        private static void WriteToLog(
            object obj,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null,
            [CallerFilePath] string filePath = null)
        {
            var traceString = caller + " at line " + lineNumber + " (" + filePath + ")";

            var time = s_Stopwatch.Elapsed;
            var timeString =
                string.Format("[{0:00}:{1:00}:{2:000}] - ", time.Minutes, time.Seconds, time.Milliseconds);

            using (var file = new StreamWriter("Log\\log.txt", true))
            {
                file.WriteLine(timeString + traceString);
                file.WriteLine(obj);
                file.WriteLine();
            }
        }
    }
}
