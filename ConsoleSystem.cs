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

        public static string OutputPath = "./output.log";

        private static Stack<string> ConsoleInputLines = new Stack<string>();

        public override void OnUpdate()
        {
            if (OnConsoleInput != null && ConsoleInputLines.Count != 0)
            {
                string line = ConsoleInputLines.Pop();
                OnConsoleInput(line);
            }
        }

        internal static void Run()
        {
            while (Framework.IsWork)
            {
                string line = Console.ReadLine();
                ConsoleInputLines.Push(line);
            }
        }

        public static void Log(string _format, params object[] _args)
        {
            string line = string.Format($"[{DateTime.Now:HH:mm:ss fff}]: " + _format, _args);
            Console.WriteLine(line);
            OnReceivedLog?.Invoke(line);
            File.AppendAllText(OutputPath, "\n" + line);
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