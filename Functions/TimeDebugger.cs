using System;
using System.Collections.Generic;

namespace SapphireEngine.Functions
{
    public class TimeDebugger : IDisposable
    {
        private static Dictionary<string, double> ListWorkTimes = new Dictionary<string, double>();

        public static void Clear() => ListWorkTimes.Clear();

        public static void Show()
        {
            ConsoleSystem.LogWarning("[TimeDebugger]: Total debug times:");
            double TotalTime = 0;
            string maxLenName = "";
            foreach (var row in ListWorkTimes)
                if (maxLenName.Length < row.Key.Length)
                    maxLenName = row.Key;
            
            foreach (var row in ListWorkTimes)
            {
                TotalTime += row.Value;
                string line = row.Key + ":";
                for (int i = 0; i < maxLenName.Length - row.Key.Length; ++i)
                    line += ' ';
                ConsoleSystem.LogWarning($"{line} [{row.Value:F3} sec]");
            }
            ConsoleSystem.LogWarning($"[TimeDebugger]: Total work time: [{TotalTime:F3} sec]");
        }
        
        
        
        public DateTime DateStartTime;
        public double TotalSecondEndTime;
        public float WarningTime;
        public string Name;
        
        
        public TimeDebugger(string name, float timeWarning = 1f)
        {
            this.Name = name;
            this.WarningTime = timeWarning;
            this.DateStartTime = DateTime.Now;  
        }
        
        
        public void Dispose()
        {
            this.TotalSecondEndTime = DateTime.Now.Subtract(this.DateStartTime).TotalSeconds;
            ListWorkTimes[this.Name] = (ListWorkTimes.ContainsKey(this.Name) ? ListWorkTimes[this.Name] + this.TotalSecondEndTime : this.TotalSecondEndTime);
            
            if (this.TotalSecondEndTime > this.WarningTime)
                ConsoleSystem.LogWarning($"[TimeDebugger]: <{this.Name}> work: [{this.TotalSecondEndTime:F3} sec] > [{this.WarningTime} sec]");
        }
    }
}