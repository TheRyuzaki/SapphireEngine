using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            BaseServer = new NetworkServer(new NetworkConfiguration(0x00) { ServerPort = 10015 });
            BaseServer.OnConnected = connection =>
            {
                ConsoleSystem.Log("BaseServer.OnConnected");
            };
            BaseServer.OnDisconnected = (connection, s) =>
            {
                ConsoleSystem.Log("BaseServer.OnDisconnected: " + s);
            };
            BaseServer.OnMessage = connection =>
            {
                int len = connection.Peer.Read.Int32();
                byte[] buffer = connection.Peer.Read.Bytes(len);
                
                connection.Peer.Write.Start();
                connection.Peer.Write.Int32((int)len);
                connection.Peer.Write.Bytes(buffer);
                connection.Peer.Write.SendTo(connection);
            };
            BaseServer.Start();
            
            BaseClient = new NetworkClient(new NetworkConfiguration(0x00));
            BaseClient.OnConnected = connection =>
            {
                ConsoleSystem.Log("BaseClient.OnConnected");
                
                Graphics graph = null;
                var bmp = new Bitmap(1920, 1080);
                graph = Graphics.FromImage(bmp);
                graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Jpeg);
                
                
                connection.Peer.Write.Start();
                connection.Peer.Write.Int32((int)ms.Length);
                connection.Peer.Write.Bytes(ms.ToArray());
                connection.Peer.Write.SendTo(connection);
            };
            BaseClient.OnDisconnected = (connection, s) =>
            {
                ConsoleSystem.Log("BaseClient.OnDisconnected: " + s);
            };
            BaseClient.OnMessage = connection =>
            {
                ConsoleSystem.Log("BaseServer.OnMessage: " + connection.Peer.Read.Length);
            };
            this.BaseClient.Connect("127.0.0.1", 10015);

               
        }
        
        public void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append("0x"+b.ToString("x8") + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }

        public override void OnUpdate()
        {
            this.BaseServer.Cycle();
            this.BaseClient.Cycle();

            Console.Title = "FPS: " + (int)(1 / DeltaTime);
        }

        public override void OnDestroy()
        {
            ConsoleSystem.Log("OnDestroy");
        }
        
        
    }
}