using System;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public IPEndPoint Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public Boolean IsConnected { get; internal set; } = false;
        public Boolean IsEncryption { get; set; } = false;
        public Int32 LastRequestTime { get; private set; }
        public Int32 LastResponseTime { get; private set; }
        public NetworkPeer Peer { get; internal set; }

        public NetworkConnection(NetworkPeer peer, IPEndPoint addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.Peer = peer;
            this.OnUpdateResponseTime();
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);

        internal void OnUpdateRequestTime(int unixtime = 0)
        {
            this.LastRequestTime = ((unixtime != 0) ? unixtime : (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        internal void OnUpdateResponseTime()
        {
            this.LastResponseTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}