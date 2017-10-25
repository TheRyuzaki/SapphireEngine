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
            this.BaseServer.OnConnected = OnConnected;
            this.BaseServer.OnMessage = OnMessage;
            this.BaseServer.Start();
        }

        private void OnMessage(NetworkConnection connection)
        {
            ConsoleSystem.Log("OnMessage");
            ConsoleSystem.Log("0: " +  connection.Peer.Read.Byte());
            ConsoleSystem.Log("1: " +  connection.Peer.Read.UInt64());
            ConsoleSystem.Log("2: " +  connection.Peer.Read.String());
            ConsoleSystem.Log("3: " +  connection.Peer.Read.Int32());
            ConsoleSystem.Log("4: " +  connection.Peer.Read.Int32());
            ConsoleSystem.Log("5: " +  connection.Peer.Read.String());
            ConsoleSystem.Log("6: " +  connection.Peer.Read.String());
            
        }

        private void OnConnected(NetworkConnection networkConnection)
        {
            ConsoleSystem.Log("OnConnected");
        }

        public override void OnUpdate()
        {
            this.BaseServer.Cycle();
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
    }
}