using System;
using System.Collections.Generic;
using System.IO;

namespace SapphireEngine
{
    public class ConsoleSystem : SapphireType
    {
        public delegate void Handler_OnConsoleInput(string line);
        public static event Handler_OnConsoleInput OnConsoleInput;
        public static event Handler_OnConsoleInput OnReceivedLog;

        private static Queue<string> ConsoleInputLines = new Queue<string>();

        public override void OnUpdate()
        {
            while (ConsoleInputLines.Count > 0)
                OnConsoleInput?.Invoke(ConsoleInputLines.Dequeue());
        }

        internal static void Run()
        {
            while (Framework.IsWork)
            {
                string line = Console.ReadLine();
                ConsoleInputLines.Enqueue(line);
            }
        }

        public static void Log(string _format, params object[] _args)
        {
            string line = string.Format($"[{DateTime.Now:HH:mm:ss fff}]: " + _format, _args);
            Console.WriteLine(line);
            OnReceivedLog?.Invoke(line);
            File.AppendAllText("./output.log", "\n" + line);
        }

        public static void LogWarning(string _format, params object[] _args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(_format, _args);
            Console.ResetColor();
        }

        public static void LogError(string _format, params object[] _args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(_format, _args);
            Console.ResetColor();
        }
    }
}