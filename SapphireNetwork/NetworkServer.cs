using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SapphireNetwork
{
    public class NetworkServer : NetworkPeer
    {
        public IPEndPoint ListenerEndPoint { get; private set; }
        public bool IsListening => this.Status;

        public Dictionary<IPEndPoint, NetworkConnection> ListConnections => m_listconnections;
        
        public NetworkServer(NetworkConfiguration configuration) : base(configuration)
        {
            
        }

        public void Kick(NetworkConnection connection, string reasone) => this.KickConnection(connection, reasone);
        
        public bool Start()
        {
            if (this.Status)
            {
                Console.WriteLine("Starting server has been failed: Server is started!");
                return false;
            }
            try
            {
                this.ListenerEndPoint = new IPEndPoint(IPAddress.Parse(this.Configuration.ServerIP), this.Configuration.ServerPort);
                this.BaseSocket = new UdpClient(this.ListenerEndPoint);
                //this.BaseSocket.Client.Bind(this.ListenerEndPoint);
                this.Status = true;
                return true;
            }
            catch (SocketException exception)
            {
                Console.WriteLine($"Socket bind from <{this.Configuration.ServerIP}:{this.Configuration.ServerPort}> has been failed: " + exception.Message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Exception from <{this.Configuration.ServerIP}> parsing host addres: " + exception.Message);
            }
            this.Status = true;
            this.Stop();
            return false;
        }

        public bool Stop()
        {
            if (!this.Status)
            {
                Console.WriteLine("Stoping server has been failed: Server is not started!");
                return false;
            }
            try
            {
                this.Status = false;
                this.BaseSocket?.Close();
                this.ListenerEndPoint = null;
                this.BaseSocket = null;
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Stoping server has been failed:" + exception.Message);
            }
            return false;
        }
    }
}