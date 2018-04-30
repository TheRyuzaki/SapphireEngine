using System;
using System.Threading;
using SapphireEngine.Struct;

namespace SapphireEngine
{
    public class Framework
    {
        public static bool IsWork { get; internal set; } = true;
        public static SapphireType Bootstraper { get; internal set; }
        
        
        public static int CountActiveTypes => FrameworkWorker.ListActiveSapphireTypes.Count;
        public static DateTime StartTimeApplication { get; internal set; } =  DateTime.Now;
        
        internal static Native.HandlerOnShotdown OnApplicationShotdown;

        public static int FPSLimit
        {
            get => 1000 / m_fpsmicrotime;
            set => m_fpsmicrotime = 1000 / value;
        }

        internal static int m_fpsmicrotime = 10;

        public static void Initialization<T>(bool _consoleApplication = true)
        {
            Console.WriteLine("Framework created by TheRyuzaki");
            
            OnApplicationShotdown += RunShutdown;
            Native.SetSignalHandler(OnApplicationShotdown, true);
            
            object bootstraper = null;
            try
            {
                bootstraper = Activator.CreateInstance(typeof(T), true);
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError($"Error to Framework.Initialization(), Error to creating <{typeof(T).Name}> type: " + ex.Message);
                return;
            }
            if (bootstraper is SapphireType)
            {
                 Bootstraper = bootstraper as SapphireType;
                 Bootstraper.RunAwake(true);
                if (_consoleApplication)
                {
                    Bootstraper.AddType<ConsoleSystem>();
                    ThreadPool.QueueUserWorkItem(_ => ConsoleSystem.Run());
                }
                FrameworkWorker.Initialization();
                Environment.Exit(0);
            }
            else
            {
                ConsoleSystem.LogError($"Error to Framework.Initialization(), Type <{typeof(T).Name}> is not have nessed SapphireType!");
                return;
            }
        }

        public static void RunToMainThread(CallBackObject _cbo) => FrameworkWorker.ListCallBackObjects.Enqueue(_cbo);
        public static void RunToMainThread(Action<object> _callBack, object _obj) => RunToMainThread(CallBackObject.GetPoolObject(_callBack, _obj ));
        
        public static void Quit() => RunShutdown();
        
        internal static void RunShutdown(int _quitcode = 0)
        {
            if (Framework.IsWork)
            {
                Framework.IsWork = false;
                if (Bootstraper != null)
                {
                    Bootstraper.BroadcastMessage("OnShutdown");
                    Bootstraper.Dispose();
                }
            }
        }
    }
}