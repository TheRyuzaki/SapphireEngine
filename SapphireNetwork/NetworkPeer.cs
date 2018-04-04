using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Lidgren.Network;

namespace SapphireNetwork
{
    public class NetworkPeer
    {
        public NetworkConfiguration Configuration;
        public NetPeer BaseSocket { get; internal set; }
        public NetworkWriter Write { get; internal set; }
        public NetworkReader Read { get; internal set; }
        public bool Status { get; internal set; } = false;

        public Action<NetworkConnection> OnConnected;
        public Action<NetworkConnection, string> OnDisconnected;
        public Action<NetworkConnection> OnMessage;
        
        internal Dictionary<NetConnection,NetworkConnection> m_listconnections = new Dictionary<NetConnection,NetworkConnection>();
        
        public NetworkPeer(NetworkConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Write = new NetworkWriter(this);
            this.Read = new NetworkReader(this);
        }

        internal void KickConnection(NetworkConnection connection, string reasone)
        {
            if (connection.IsConnected)
                if (this.m_listconnections.TryGetValue(connection.Addres, out NetworkConnection connectionKick))
                    connectionKick.Addres.Disconnect(reasone);
        }
        

        public virtual void Cycle()
        {
            if (this.Status)
            {
                NetIncomingMessage message;
                while ((message = this.BaseSocket.ReadMessage()) != null)
                {
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus) message.ReadByte();
                            if (status == NetConnectionStatus.Connected)
                            {
                                NetworkConnection connection;
                                if (this.m_listconnections.TryGetValue(message.SenderConnection, out connection) == false)
                                    connection = new NetworkConnection(this, message.SenderConnection);
                                this.m_listconnections[message.SenderConnection] = connection;
                                connection.IsConnected = true;
                                this?.OnConnected(connection);
                            }
                            else if (status == NetConnectionStatus.Disconnected)
                            {
                                if (this.m_listconnections.TryGetValue(message.SenderConnection, out NetworkConnection statusConnection))
                                {
                                    string reasone = message.ReadString();
                                    if (this.m_listconnections.ContainsKey(statusConnection.Addres))
                                        this.m_listconnections.Remove(statusConnection.Addres);
                                    statusConnection.IsConnected = false;
                                    if (this is NetworkClient)
                                        this.Status = false;
                                    this?.OnDisconnected(statusConnection, reasone);
                                    statusConnection.Addres = null;
                                }
                            }

                            break;
                        case NetIncomingMessageType.Data:
                            if (this.m_listconnections.TryGetValue(message.SenderConnection, out NetworkConnection dataConnection))
                            {
                                int bufferLen = message.ReadInt32();
                                byte[] buffer = message.ReadBytes(bufferLen);

                                if (this.Configuration.Cryptor != null)
                                    buffer = this.Configuration.Cryptor.Decryption(buffer);

                                this.Read.Buffer = buffer;
                                this?.OnMessage(dataConnection);
                            }

                            break;
                    }

                    this.BaseSocket.Recycle(message);
                }

            }
        }
    }
}