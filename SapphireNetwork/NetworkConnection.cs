using System;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public IPEndPoint Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public bool IsConnected { get; internal set; } = false;
        public int LastRequestTime { get; private set; }
        public int LastResponseTime { get; private set; }
        public NetworkPeer Peer { get; internal set; }

        public NetworkConnection(NetworkPeer peer, IPEndPoint addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.Peer = peer;
            this.OnUpdateResponseTime();
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);

        internal void OnUpdateRequestTime() => this.LastRequestTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        internal void OnUpdateResponseTime() => this.LastResponseTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
    }
}