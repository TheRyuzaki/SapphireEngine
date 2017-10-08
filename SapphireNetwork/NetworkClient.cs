using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SapphireNetwork
{
    public class NetworkClient : NetworkPeer
    {
        public IPEndPoint ConnectedEndPoint;
        public bool IsConnected => this.Status && this.m_listconnections.Count != 0;
        public NetworkConnection Connection => this.m_listconnections.FirstOrDefault().Value;

        private bool m_connection_status = false;
        
        public NetworkClient(NetworkConfiguration configuration) : base(configuration)
        {
            
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
                this.BaseSocket = new UdpClient();
                this.BaseSocket.Client.SendTo(new byte[] {253, 253, 253, 253, this.Configuration.IndeficationByte}, this.ConnectedEndPoint);
                this.Status = true;
                return true;
            }
            catch (Exception exception)
            {
                this.Status = true;
                this.Disconnect();
                Console.WriteLine($"Connecting to <{host}:{port}> has been failed: " + exception.Message);
            }
            return false;
        }

        public bool Disconnect()
        {
            
            if (!this.Status)
            {
                Console.WriteLine("Disconnecting has been failed: You dont have connection!");
                return false;
            }
            try
            {
                if (this.IsConnected)
                    this.BaseSocket?.Client.SendTo(new byte[] { 254, 254, 254, 254}, this.ConnectedEndPoint);
                OnDisconnected?.Invoke(this.Connection, "Disconnected");
                this.Status = false;
                this.BaseSocket?.Close();
                this.BaseSocket = null;
                this.ConnectedEndPoint = null;
                this.m_listconnections.Clear();
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