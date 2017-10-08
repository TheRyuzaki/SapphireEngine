using System;
using System.Net;

namespace SapphireNetwork
{
    public struct NetworkReceivedPacket
    {
        public IPEndPoint Addres;
        public Byte[] Buffer;
    }
}