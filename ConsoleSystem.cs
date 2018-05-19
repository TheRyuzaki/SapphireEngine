using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SapphireEngine
{
    public class ConsoleSystem : SapphireType
    {
        public delegate void Handler_OnConsoleInput(string line);
        public static event Handler_OnConsoleInput OnConsoleInput;
        public static event Handler_OnConsoleInput OnReceivedLog;

        public static string OutputPath = "./output.log";
        public static bool IsOutputToFile = true;

        public static bool ShowCallerInLog = false;
        
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

        private static void Output(string _format, params object[] _args)
        {
            string prefix = "";
            if (ShowCallerInLog)
            {
                StackFrame frame = new StackFrame(2, true);
                var method = frame.GetMethod();
                var declaringType = method.DeclaringType.Name;
                if (declaringType.StartsWith("<>"))
                    declaringType = method.DeclaringType.DeclaringType.Name;
                prefix = $"[{declaringType}.{method}]";
            }
            string line = string.Format($"[{DateTime.Now:HH:mm:ss fff}]{prefix}: " + _format, _args);
            Console.WriteLine(line);
            OnReceivedLog?.Invoke(line);
            if (IsOutputToFile)
                File.AppendAllText(OutputPath, "\n" + line);
        }

        public static void Log(string _format, params object[] _args)
        {
            Output(_format, _args);
        }

        public static void LogWarning(string _format, params object[] _args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Output(_format, _args);
            Console.ResetColor();
        }

        public static void LogError(string _format, params object[] _args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Output(_format, _args);
            Console.ResetColor();
        }
    }
}