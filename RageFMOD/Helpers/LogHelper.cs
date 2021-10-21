using FMOD;
using System;
using System.IO;

namespace RageAudio.Helpers
{
    internal static class LogHelper
    {
        /// <summary>
        /// Name of the file with log.
        /// </summary>
        private const string logFileName = "RageAudio.log";

        /// <summary>
        /// Clears log.
        /// </summary>
        internal static void Clear()
        {
            File.WriteAllText(logFileName, string.Empty);
        }

        /// <summary>
        /// Logs fmod function result to RageAudio.log file in format of: Time, BankDirectory, Action, Result
        /// </summary>
        /// <param name="audioPlayer">AudioPlayer context.</param>
        /// <param name="action">Action, result of which needs to be logged. For Example - "System Init".</param>
        /// <param name="result">Result of the action.</param>
        internal static void Log(AudioPlayer audioPlayer, string action, RESULT result)
        {
            File.AppendAllText(logFileName, $"{DateTime.Now}, {audioPlayer.BankDirectory}, {action}, {result}\n");
        }

        /// <summary>
        /// Logs fmod function result to RageAudio.log file in format of: Time, Action, Result
        /// </summary>
        /// <param name="action">Action, result of which needs to be logged. For Example - "System Init".</param>
        /// <param name="result">Result of the action.</param>
        internal static void Log(string action, RESULT result)
        {
            File.AppendAllText(logFileName, $"{DateTime.Now}, {action}, {result}\n");
        }

        /// <summary>
        /// Logs fmod function result to RageAudio.log file in format of: Time, Action, Result
        /// </summary>
        /// <param name="action">Action, result of which needs to be logged. For Example - "System Init".</param>
        /// <param name="result">Result of the action.</param>
        internal static void Log(string action, string result)
        {
            File.AppendAllText(logFileName, $"{DateTime.Now}, {action}, {result}\n");
        }

        /// <summary>
        /// Logs memory address to RageAudio.log file in format of: Time, $name Found At $ptr
        /// </summary>
        /// <param name="ptr">Pointer to log.</param>
        /// <param name="name">Name to log.</param>
        internal static void Log(IntPtr ptr, string name)
        {
            File.AppendAllText(logFileName, $"{DateTime.Now}, {name} Found At: {ptr.ToString("X")}\n");
        }
    }
}
