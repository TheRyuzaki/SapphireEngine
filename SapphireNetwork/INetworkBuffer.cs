using System;
using System.IO;

namespace SapphireNetwork
{
    public interface INetworkBuffer
    {
        NetworkPeer Peer { get; }
        Byte[] Buffer { get; set; }
        Int32 Length { get; }
        Int32 Position { get; set; }
    }
}