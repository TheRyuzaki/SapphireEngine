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
        public NetworkConnection Connection => this.m_listconnections.Count != 0 ? this.m_listconnections.ElementAt(0).Value : null;

        private int m_startConnectionTime = 0;
        private int m_countFailedTick = 0;
        
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
                this.BaseSocket.Client.SendTo(new byte[] {253, 253, 253, 253}, this.ConnectedEndPoint);
                this.m_startConnectionTime = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                this.m_countFailedTick = 1;
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

        public override void Cycle()
        {
            base.Cycle();
            if (this.Status == true && this.m_listconnections.Count == 0)
            {
                if ((Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds == this.m_startConnectionTime + this.m_countFailedTick)
                {
                    if (this.m_countFailedTick < this.Configuration.TimeOut)
                    {
                        this.m_countFailedTick++;
                        this.BaseSocket.Client.SendTo(new byte[] {253, 253, 253, 253}, this.ConnectedEndPoint);
                    }
                    else
                        this.Disconnect("Time Out!");
                }
            }
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
                if (this.IsConnected)
                    this.BaseSocket?.Client.SendTo(new byte[] { 254, 254, 254, 254}, this.ConnectedEndPoint);
                var connection = this.Connection;
                this.Status = false;
                this.BaseSocket?.Close();
                this.BaseSocket = null;
                this.m_startConnectionTime = 0;
                this.m_countFailedTick = 1;
                this.ConnectedEndPoint = null;
                this.m_listconnections.Clear();
                OnDisconnected?.Invoke(connection, reasone);
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