using System;
using System.Collections.Generic;
using System.IO;

namespace Ryne.Utility
{
    public enum LogCategory
    {
        Normal,
        Warning,
        Error
    }

    public class Logger
    {
        // List of messages that have to be displayed in editor
        public static List<Tuple<LogCategory, string>> CurrentLog;
        // List with messages that have to be written to file
        private static List<string> ToFlush;

        public static void Initialize(string filename = "Log.txt")
        {
            CurrentLog = new List<Tuple<LogCategory, string>>();
            ToFlush = new List<string>();

            // Empty log file so this new instance appends to a fresh file
            Flush(filename, FileMode.Create);
        }

        public static Float4 GetColor(LogCategory category)
        {
            switch (category)
            {
                case LogCategory.Warning: return new Float4(1.0f, 1.0f, 0.0f, 1.0f);
                case LogCategory.Error: return new Float4(1.0f, 0.0f, 0.0f, 1.0f);
                default: return new Float4(1.0f);
            }
        }

        public static void Log(string message, LogCategory category = LogCategory.Normal)
        {
            var prefix = category != LogCategory.Normal ? category.ToString() : "";
            prefix = !string.IsNullOrEmpty(prefix) ? prefix + ": " : "";
            message = prefix + message;
            CurrentLog.Add(new Tuple<LogCategory, string>(category, message));
            ToFlush.Add(message);
        }

        public static void Warning(string message)
        {
            Log(message, LogCategory.Warning);
            Flush();
        }

        public static void Error(string message)
        {
            Log(message, LogCategory.Error);
            Flush();
#if DEBUG
            throw new Exception(message);
#endif
        }

        // Writes log in file
        public static void Flush(string filename = "Log.txt", FileMode mode = FileMode.Append)
        {
            TextWriter writer = new StreamWriter(new BufferedStream(new FileStream(filename, mode)));
            foreach (var message in ToFlush)
            {
                writer.WriteLine(message);
            }
            writer.Flush();
            writer.Close();
            ToFlush.Clear();
        }
    }
}
