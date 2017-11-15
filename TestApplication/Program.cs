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
        private NetworkClient BaseClient;
        
        
        public override void OnAwake()
        {
            this.BaseServer.OnConnected = OnConnected;
            this.BaseServer.OnDisconnected = OnDisconnected;
            this.BaseServer.Start();
            
            BaseClient = new NetworkClient(new NetworkConfiguration() );
            BaseClient.OnConnected = connection => { ConsoleSystem.Log("OnConnected Client"); };
            BaseClient.OnDisconnected = (connection, s) => { ConsoleSystem.Log("Disconnected client reasone: " + s); };
            BaseClient.Connect("127.0.0.1", 10015);
        }

        private void OnDisconnected(NetworkConnection networkConnection, string s)
        {
            ConsoleSystem.Log("Disconnected reasone: " + s);
        }

        private void OnConnected(NetworkConnection networkConnection)
        {
            ConsoleSystem.Log("OnConnected");
        }

        public override void OnUpdate()
        {
            if (BaseServer != null)
            {
                this.BaseServer.Cycle();
            }
            if (this.BaseClient != null)
            {
                this.BaseClient.Cycle();
            }
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
    }
}