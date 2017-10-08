using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SapphireEngine;
using SapphireEngine.Functions;
using SapphireNetwork;

namespace TestApplication
{
    internal class Program : SapphireType
    {
        public static void Main() => Framework.Initialization<Program>(true);

        NetworkServer BaseServer = new NetworkServer(new NetworkConfiguration());
        
        
        public override void OnAwake()
        {
            BaseServer.OnQueryRequest = OnQueryRequest;
        }

        private void OnQueryRequest(IPEndPoint ipEndPoint, byte[] bytes)
        {
            
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
    }
}