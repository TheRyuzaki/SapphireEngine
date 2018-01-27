using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SapphireNetwork
{
    public class NetworkPeer
    {
        public NetworkConfiguration Configuration;
        public UdpClient BaseSocket { get; internal set; }
        public NetworkWriter Write { get; internal set; }
        public NetworkReader Read { get; internal set; }
        public bool Status { get; internal set; } = false;

        public Action<NetworkConnection> OnConnected;
        public Action<IPEndPoint, Byte[]> OnQueryRequest;
        public Action<NetworkConnection, string> OnDisconnected;
        public Action<NetworkConnection> OnMessage;
        
        public Queue<NetworkReceivedPacket> ListStackPackets = new Queue<NetworkReceivedPacket>();
        internal Dictionary<IPEndPoint,NetworkConnection> m_listconnections = new Dictionary<IPEndPoint,NetworkConnection>();
        internal Dictionary<NetworkConnection, string> m_listdisconnected = new Dictionary<NetworkConnection, string>();
        
        public NetworkPeer(NetworkConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Write = new NetworkWriter(this);
            this.Read = new NetworkReader(this);
            ThreadPool.QueueUserWorkItem(Receiving);
        }

        private void Receiving(object sender)
        {
            IPEndPoint endpointLastsender = new IPEndPoint(IPAddress.Any, 0);
            byte[] buffer = new byte[1024];
            
            while (true)
            {
                try
                {
                    if (this.Status && this.BaseSocket != null)
                    {
                        buffer = this.BaseSocket.Receive(ref endpointLastsender);
                        ListStackPackets.Enqueue(new NetworkReceivedPacket {Buffer = buffer, Addres = (IPEndPoint) endpointLastsender});
                    }
                    else
                        Thread.Sleep(10);
                }
                catch { }
            }
        }

        internal void KickConnection(NetworkConnection connection, string reasone)
        {
            if (connection.IsConnected)
            {
                connection.IsConnected = false;
                this.m_listdisconnected[connection] = reasone;
            }
        }

        public bool SendUnconnectedMessage(IPEndPoint addres, byte[] buffer, int bytes_len)
        {
            if (this.Status)
            {
                this.BaseSocket.Send(buffer, bytes_len, addres);
                return true;
            }
            return false;
        }
        

        public virtual void Cycle()
        {
            if (this.Status)
            {
                while (this.ListStackPackets.Count != 0)
                {
                    NetworkReceivedPacket packet = ListStackPackets.Dequeue();
                    
                    if (packet.Buffer != null && packet.Buffer.Length > 1)
                    {
                        switch (packet.Buffer[0])
                        {
                            case 255:
                                if (packet.Buffer[1] == 255 && packet.Buffer.Length > 3 && packet.Buffer[2] == 255 && packet.Buffer[3] == 255)
                                {
                                    OnQueryRequest?.Invoke(packet.Addres, packet.Buffer);
                                    continue;
                                }
                                break;
                            case 254:
                                if (packet.Buffer[1] == 254 && packet.Buffer.Length > 3 && packet.Buffer[2] == 254 && packet.Buffer[3] == 254)
                                {
                                    if (this.m_listconnections.TryGetValue(packet.Addres, out var connection))
                                    {
                                        string reasone = "Disconnected";
                                        if (packet.Buffer.Length != 4)
                                        {
                                            byte[] reasoneBuffer = new byte[packet.Buffer.Length - 4];
                                            for (int i = 4; i < packet.Buffer.Length; ++i)
                                                reasoneBuffer[i - 4] = packet.Buffer[i];
                                            reasone = Encoding.ASCII.GetString(reasoneBuffer);
                                        }
                                        this.KickConnection(connection, reasone);
                                    }
                                    continue;
                                }
                                break;
                            case 253:
                                if (packet.Buffer[1] == 253 && packet.Buffer.Length > 4 && packet.Buffer[2] == 253 && packet.Buffer[3] == 253 && packet.Buffer[4] == this.Configuration.IndeficationByte)
                                {
                                    if (this.m_listconnections.TryGetValue(packet.Addres, out _) == false)
                                    {
                                        if (this is NetworkServer)
                                            this.BaseSocket.Client.SendTo(new byte[] {253, 253, 253, 253, this.Configuration.IndeficationByte}, packet.Addres);
                                        
                                        this.m_listconnections[packet.Addres] = new NetworkConnection(this, packet.Addres);
                                        OnConnected?.Invoke(this.m_listconnections[packet.Addres]);
                                    }
                                    continue;
                                }
                                break;
                            case 252:
                                if (packet.Buffer[1] == 252 && packet.Buffer.Length > 3 && packet.Buffer[2] == 252 && packet.Buffer[3] == 252)
                                {
                                    if (this.m_listconnections.TryGetValue(packet.Addres, out var connection))
                                    {
                                        connection.OnUpdateResponseTime();
                                    }
                                    continue;
                                }
                                break;
                        }
                        
                        if (this.Configuration.Cryptor != null)
                            packet.Buffer = this.Configuration.Cryptor.Decryption(packet.Buffer);
                        
                        if (packet.Buffer[0] == this.Configuration.IndeficationByte && this.m_listconnections.TryGetValue(packet.Addres, out NetworkConnection connection_end) && this.m_listconnections[packet.Addres].IsConnected)
                        {
                            this.Read.Buffer = packet.Buffer;
                            OnMessage?.Invoke(connection_end);
                        }
                    }
                }

                if (this.m_listconnections.Count != 0)
                {
                    int thisTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    foreach (var connection in this.m_listconnections)
                    {
                        if (thisTime - connection.Value.LastResponseTime >= this.Configuration.TimeOut)
                        {
                            this.KickConnection(connection.Value, "Time Out!");
                        }
                        else if ((int) thisTime != (int) connection.Value.LastRequestTime)
                        {
                            this.BaseSocket.Client.SendTo(new byte[] {252, 252, 252, 252}, connection.Key);
                            connection.Value.OnUpdateRequestTime();
                        }
                    }

                    if (this.m_listdisconnected.Count != 0)
                    {
                        if (this is NetworkServer)
                        {
                            foreach (var connection in this.m_listdisconnected)
                            {
                                if (this.m_listconnections.TryGetValue(connection.Key.Addres, out _))
                                    this.m_listconnections.Remove(connection.Key.Addres);

                                List<Byte> disconnectBytes = new List<byte>() { 254, 254, 254, 254 };
                                disconnectBytes.AddRange(Encoding.ASCII.GetBytes(connection.Value));
                                
                                this.BaseSocket.Client.SendTo(disconnectBytes.ToArray(), connection.Key.Addres);
                                OnDisconnected?.Invoke(connection.Key, connection.Value);
                            }
                        }
                        else
                            (this as NetworkClient).Disconnect(this.m_listdisconnected.First().Value);
                        this.m_listdisconnected.Clear();
                    }

                }
            }
        }
    }
}