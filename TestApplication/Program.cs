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
            BaseServer = new NetworkServer(new NetworkConfiguration(0x00) { ServerPort = 10015 });
            BaseServer.Configuration.Cryptor = new NCrypt();
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
                ConsoleSystem.Log("BaseServer.OnMessage: " + connection.Peer.Read.Boolean());
            };
            BaseServer.Start();
            
            BaseClient = new NetworkClient(new NetworkConfiguration(0x00));
            BaseClient.Configuration.Cryptor = new NCrypt();
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
                ConsoleSystem.Log("BaseClient.OnMessage: " + connection.Peer.Read.Boolean());
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
        
        public class NCrypt : INetworkCryptor
        {
            public byte[] Encryption(byte[] buffer) => RC4.Run(new byte[] { 0x1F, 0x2F, 0x1B, 0x2B }, buffer).ToArray();

            public byte[] Decryption(byte[] buffer) => RC4.Run(new byte[] { 0x1F, 0x2F, 0x1B, 0x2B }, buffer).ToArray();
        
            public static class RC4
            {
                private static byte[] Init(byte[] key)
                {
                    byte[] s = Enumerable.Range(0, 256)
                        .Select(i => (byte)i)
                        .ToArray();

                    for (int i = 0, j = 0; i < 256; i++)
                    {
                        j = (j + key[i % key.Length] + s[i]) & 255;

                        Swap(s, i, j);
                    }

                    return s;
                }

                public static IEnumerable<byte> Run(byte[] key, IEnumerable<byte> data)
                {
                    byte[] s = Init(key);

                    int i = 0;
                    int j = 0;

                    return data.Select((b) =>
                    {
                        i = (i + 1) & 255;
                        j = (j + s[i]) & 255;

                        Swap(s, i, j);

                        return (byte)(b ^ s[(s[i] + s[j]) & 255]);
                    });
                }

                private static void Swap(byte[] s, int i, int j)
                {
                    byte c = s[i];

                    s[i] = s[j];
                    s[j] = c;
                }
            }
        }
    }
}