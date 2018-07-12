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
    public class Crypto : INetworkCryptor
    {
        private byte[] BufferKey = new byte[] { 0xB1, 0x2B, 0xF1 }; 
        
        public byte[] Encryption(byte[] buffer)
        {
            return RC4.Encrypt(this.BufferKey, buffer);
        }

        public byte[] Decryption(byte[] buffer)
        {
            return  RC4.Decrypt(this.BufferKey, buffer);
        }
        
        public class RC4 {

            public static byte[] Encrypt(byte[] pwd, byte[] data) {
                int a, i, j, k, tmp;
                int[] key, box;
                byte[] cipher;

                key = new int[256];
                box = new int[256];
                cipher = new byte[data.Length];

                for (i = 0; i < 256; i++) {
                    key[i] = pwd[i % pwd.Length];
                    box[i] = i;
                }
                for (j = i = 0; i < 256; i++) {
                    j = (j + box[i] + key[i]) % 256;
                    tmp = box[i];
                    box[i] = box[j];
                    box[j] = tmp;
                }
                for (a = j = i = 0; i < data.Length; i++) {
                    a++;
                    a %= 256;
                    j += box[a];
                    j %= 256;
                    tmp = box[a];
                    box[a] = box[j];
                    box[j] = tmp;
                    k = box[((box[a] + box[j]) % 256)];
                    cipher[i] = (byte)(data[i] ^ k);
                }
                return cipher;
            }

            public static byte[] Decrypt(byte[] pwd, byte[] data) {
                return Encrypt(pwd, data);
            }

        }
    }
    
    internal class Program : SapphireType
    {
        public static void Main() => Framework.Initialization<Program>(true);

        private NetworkServer BaseServer;
        private NetworkClient BaseClient;
        
        public override void OnAwake()
        {
            ConsoleSystem.ShowCallerInLog = false;
            BaseServer = new NetworkServer(new NetworkConfiguration(0x00) { ServerPort = 10015, Cryptor = new Crypto()});
            BaseServer.OnConnected = connection =>
            {
                connection.IsEncryption = true;
                ConsoleSystem.Log("BaseServer.OnConnected");
            };
            BaseServer.OnDisconnected = (connection, s) =>
            {
                ConsoleSystem.Log("BaseServer.OnDisconnected: " + s);
            };
            BaseServer.OnMessage = connection =>
            {
                ConsoleSystem.Log("BaseServer.OnMessage => " + connection.Peer.Read.String());
                
//                connection.Peer.Write.Start();
//                connection.Peer.Write.String("Hello");
//                connection.Peer.Write.SendTo(connection);
            };
            BaseServer.Start();
            
            BaseClient = new NetworkClient(new NetworkConfiguration(0x00){Cryptor = new Crypto()});
//            BaseClient.OnConnected = connection =>
//            {
//                connection.IsEncryption = true;
//                ConsoleSystem.Log("BaseClient.OnConnected");
//                
//                connection.Peer.Write.Start();
//                connection.Peer.Write.String("Hello");
//                connection.Peer.Write.SendTo(connection);
//            };
//            BaseClient.OnDisconnected = (connection, s) =>
//            {
//                ConsoleSystem.Log("BaseClient.OnDisconnected: " + s);
//            };
//            BaseClient.OnMessage = connection =>
//            {
//                ConsoleSystem.Log("BaseServer.OnMessage => " + connection.Peer.Read.String());
//            };
//            this.BaseClient.Connect("127.0.0.1", 10015);
//
//
//            SapphireEngine.Functions.Timer.SetTimeout(() =>
//            {
//                SapphireEngine.Functions.Timer.SetTimeout(() => { SapphireEngine.Functions.Timer.SetTimeout(() => { }, 10);}, 10);
//                
//            }, 10);

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