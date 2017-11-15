using System;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public IPEndPoint Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public bool IsConnected { get; internal set; } = false;
        public double LastSyncTime { get; private set; }
        public NetworkPeer Peer { get; internal set; }

        internal int m_listFailedSync = 1;

        public NetworkConnection(NetworkPeer peer, IPEndPoint addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.Peer = peer;
            this.LastSyncTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);

        internal void UpdateSyncTime() => this.LastSyncTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}