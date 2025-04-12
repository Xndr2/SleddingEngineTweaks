using System;
using System.IO;

namespace SleddingGameModFramework.Utils
{
    /// <summary>
    /// Logging severity levels
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Simple logging utility for the mod framework
    /// </summary>
    public static class Logger
    {
        private static readonly string LogFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Logs",
            $"mod_framework_{DateTime.Now:yyyyMMMMdd}.log");
        
        private static bool _initialized = false;

        /// <summary>
        /// Initializes the logger
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogFilePath));
                _initialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialize logger: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a message with the specified severity
        /// </summary>
        /// <param name="level">Severity level</param>
        /// <param name="message">Message to log</param>
        public static void Log(LogLevel level, string message)
        {
            try
            {
                if (!_initialized) Initialize();
                
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
                
                // Write to console for debugging
                Console.WriteLine(logMessage);
                
                // Append to log file
                File.AppendAllText(LogFilePath, logMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write log: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Logs a debug message
        /// </summary>
        public static void Debug(string message) => Log(LogLevel.Debug, message);

        /// <summary>
        /// Logs an informational message
        /// </summary>
        public static void Info(string message) => Log(LogLevel.Info, message);

        /// <summary>
        /// Logs a warning message
        /// </summary>
        public static void Warning(string message) => Log(LogLevel.Warning, message);

        /// <summary>
        /// Logs an error message
        /// </summary>
        public static void Error(string message) => Log(LogLevel.Error, message);
        
        /// <summary>
        /// Logs a critical error message
        /// </summary>
        public static void Critical(string message) => Log(LogLevel.Critical, message);
    }
}

