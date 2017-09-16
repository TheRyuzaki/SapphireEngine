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

        public static void Log(string format, params object[] args)
        {
            string line = string.Format($"[{DateTime.Now:HH:mm:ss fff}]: " + format, args);
            Console.WriteLine(line);
            OnReceivedLog?.Invoke(line);
            File.AppendAllText("./output.log", "\n" + line);
        }

        public static void LogWarning(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Log(format, args);
            Console.ResetColor();
        }

        public static void LogError(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Log(format, args);
            Console.ResetColor();
        }
    }
}