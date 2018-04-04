using System;
using System.Net;
using Lidgren.Network;

namespace SapphireNetwork
{
    public class NetworkConnection
    {
        public NetConnection Addres { get; internal set; }
        public object ConnectionProfile = null;
        
        public bool IsConnected { get; internal set; } = false;
        public NetworkPeer Peer { get; internal set; }

        public NetworkConnection(NetworkPeer peer, NetConnection addres)
        {
            this.Addres = addres;
            this.IsConnected = true;
            this.Peer = peer;
        }

        public void Disconnect(string reasone) => this.Peer.KickConnection(this, reasone);
    }
}