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

        private NetworkServer BaseServer;
        private NetworkClient BaseClient;
        
        public override void OnAwake()
        {
            BaseServer = new NetworkServer(new NetworkConfiguration() { ServerPort = 10015 });
            BaseServer.OnConnected = connection =>
            {
                ConsoleSystem.Log("BaseServer.OnConnected");
                connection.Peer.Write.Start();
                connection.Peer.Write.Boolean(true);
                connection.Peer.Write.SendToAll();
            };
            BaseServer.OnDisconnected = (connection, s) =>
            {
                ConsoleSystem.Log("BaseServer.OnDisconnected: " + s);
            };
            BaseServer.OnMessage = connection =>
            {
                //ConsoleSystem.Log("BaseServer.OnMessage: " + connection.Peer.Read.Length);
            };
            BaseServer.Start();
            
            BaseClient = new NetworkClient(new NetworkConfiguration());
            BaseClient.OnConnected = connection =>
            {
                ConsoleSystem.Log("BaseClient.OnConnected");
            };
            BaseClient.OnDisconnected = (connection, s) =>
            {
                ConsoleSystem.Log("BaseClient.OnDisconnected: " + s);
            };
            BaseClient.OnMessage = connection =>
            {
                //ConsoleSystem.Log("BaseClient.OnMessage: " + connection.Peer.Read.Length);
            };
            this.BaseClient.Connect("127.0.0.1", 10015);

        }

        public override void OnUpdate()
        {
            this.BaseServer.Cycle();
            this.BaseClient.Cycle();
            
            this.BaseServer.Write.Start();
            this.BaseServer.Write.Boolean(true);
            this.BaseServer.Write.SendToAll();
            Console.Title = "FPS: " + (int)(1 / DeltaTime);
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
    }
}