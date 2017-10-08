using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
        
        public Stack<NetworkReceivedPacket> ListStackPackets = new Stack<NetworkReceivedPacket>();
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
                        ListStackPackets.Push(new NetworkReceivedPacket {Buffer = buffer, Addres = (IPEndPoint) endpointLastsender});
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
        

        public void Cycle()
        {
            if (this.Status)
            {
                while (this.ListStackPackets.Count != 0)
                {
                    NetworkReceivedPacket packet = ListStackPackets.Pop();
                    
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
                                    if (this.m_listconnections.ContainsKey(packet.Addres))
                                        this.KickConnection(this.m_listconnections[packet.Addres], "Disconnected");
                                    continue;
                                }
                                break;
                            case 253:
                                if (packet.Buffer[1] == 253 && packet.Buffer.Length > 4 && packet.Buffer[2] == 253 && packet.Buffer[3] == 253 && packet.Buffer[4] == this.Configuration.IndeficationByte)
                                {
                                    if (this.m_listconnections.ContainsKey(packet.Addres) == false)
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
                                    if (this.m_listconnections.ContainsKey(packet.Addres))
                                    {
                                        this.m_listconnections[packet.Addres].SyncSended = false;
                                        this.m_listconnections[packet.Addres].LastSyncTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                                        if (this is NetworkServer)
                                            this.BaseSocket.Client.SendTo(new byte[] {252, 252, 252, 252}, packet.Addres);
                                    }
                                    continue;
                                }
                                break;
                        }
                        
                        if (packet.Buffer[0] == this.Configuration.IndeficationByte && this.m_listconnections.ContainsKey(packet.Addres) && this.m_listconnections[packet.Addres].IsConnected)
                        {
                            this.Read.Buffer = packet.Buffer;
                            this.m_listconnections[packet.Addres].LastSyncTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                            OnMessage?.Invoke(this.m_listconnections[packet.Addres]);
                        }
                    }
                }

                if (this.m_listconnections.Count != 0)
                {
                    double thisTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                    foreach (var connection in this.m_listconnections)
                    {
                        if (thisTime - connection.Value.LastSyncTime >= this.Configuration.TimeOut)
                        {
                            this.KickConnection(connection.Value, "Time Out!");
                        }
                        else if (this is NetworkClient && thisTime - connection.Value.LastSyncTime >= this.Configuration.TimeOut - 5 && connection.Value.SyncSended == false)
                        {
                            connection.Value.SyncSended = true;
                            this.BaseSocket.Client.SendTo(new byte[] {252, 252, 252, 252}, connection.Key);
                        }

                    }

                    foreach (var connection in this.m_listdisconnected)
                    {
                        if (this.m_listconnections.ContainsKey(connection.Key.Addres))
                            this.m_listconnections.Remove(connection.Key.Addres);
                        
                        this.BaseSocket.Client.SendTo(new byte[] { 254, 254, 254, 254}, connection.Key.Addres);
                        OnDisconnected?.Invoke(connection.Key, connection.Value);
                    }
                    this.m_listdisconnected.Clear();
                }
            }
        }
    }
}