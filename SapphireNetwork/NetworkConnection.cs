using System;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public IPEndPoint Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public bool IsConnected { get; internal set; } = false;
        public int LastSyncTime { get; private set; }
        public NetworkPeer Peer { get; internal set; }

        internal bool IsSyncedConnecion => this.LastSyncTime == (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

        public NetworkConnection(NetworkPeer peer, IPEndPoint addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.Peer = peer;
            this.UpdateSyncTime();
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);

        internal void UpdateSyncTime() => this.LastSyncTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}