using System;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public IPEndPoint Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public bool IsConnected { get; internal set; } = false;
        public double LastSyncTime { get; internal set; }
        public NetworkPeer Peer { get; internal set; }

        internal bool SyncSended = false;

        public NetworkConnection(NetworkPeer peer, IPEndPoint addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.LastSyncTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);
    }
}