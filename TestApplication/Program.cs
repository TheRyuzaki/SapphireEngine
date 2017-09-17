using System;
using System.Collections.Generic;
using System.IO;
using SapphireEngine;
using SapphireEngine.Functions;

namespace TestApplication
{
    internal class Program : SapphireType
    {
        public static void Main() => Framework.Initialization<Program>(true);

        public override void OnAwake()
        {
            ConsoleSystem.Log("OnAwake");
            Timer.SetTimeout(() => { ConsoleSystem.Log("After Awake 1 second"); }, 1f);
            Timer.SetInterval(() => { ConsoleSystem.Log("TickAfter 0.5 sec"); }, 0.5f);
        }
        
        private void OnShutdown()
        {
            ConsoleSystem.Log("OnShutdown");
        }

        public override void OnUpdate()
        {
//            ConsoleSystem.Log("OnUpdate");
//            Framework.Quit();
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
    }
}