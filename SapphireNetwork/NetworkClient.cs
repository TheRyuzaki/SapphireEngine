using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Lidgren.Network;

namespace SapphireNetwork
{
    public class NetworkClient : NetworkPeer
    {
        public IPEndPoint ConnectedEndPoint;
        public bool IsConnected => this.Status && this.m_listconnections.Count != 0;
        public NetworkConnection Connection => this.m_listconnections.Count != 0 ? this.m_listconnections.ElementAt(0).Value : null;

        private int m_startConnectionTime = 0;
        private int m_countFailedTick = 0;
        
        public NetworkClient(NetworkConfiguration configuration) : base(configuration)
        {
            this.BaseSocket = new NetServer(new NetPeerConfiguration(configuration.IndeficationByte.ToString())
            {
                AutoFlushSendQueue = true,
                ConnectionTimeout = configuration.TimeOut
            });
            this.BaseSocket.Start();
        }
        
        public bool Connect(string host, int port)
        {
            if (this.Status)
            {
                Console.WriteLine($"Connecting to <{host}:{port}> has been failed: You have open connection!");
                return false;
            }
            try
            {
                this.ConnectedEndPoint = new IPEndPoint(IPAddress.Parse(host), port);

                var netConnection = this.BaseSocket.Connect(this.ConnectedEndPoint);
                this.m_listconnections[netConnection] = new NetworkConnection(this, netConnection);
                this.m_listconnections[netConnection].IsConnected = false;
                this.Status = true;
                
                return true;
            }
            catch (Exception exception)
            {
                this.Status = false;
                this.Disconnect();
                Console.WriteLine($"Connecting to <{host}:{port}> has been failed: " + exception.Message);
            }
            return false;
        }


        public bool Disconnect(string reasone = "Disconnected")
        {
            if (!this.Status)
            {
                Console.WriteLine("Disconnecting has been failed: You dont have connection!");
                return false;
            }
            try
            {
                if (this.IsConnected && this.Connection != null)
                {
                    this.Connection.Disconnect(reasone);
                    for (int i = 0; i < 5; i++)
                    {
                        this.Cycle();
                        Thread.Sleep(100);
                    }
                }

                this.Status = false;
                OnDisconnected?.Invoke(this.Connection, reasone);
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Disconnecting has been failed: " + exception.Message);
            }
            return false;
        }
    }
}