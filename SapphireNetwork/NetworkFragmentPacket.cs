using System;

namespace SapphireNetwork
{
    internal class NetworkFragmentPacket : IDisposable
    {
        public int PacketID { get; private set; }
        public uint TotalBufferSize { get; private set; }
        public uint CurentBufferSize { get; private set; }
        public byte[] Buffer { get; private set; }

        public NetworkFragmentPacket(uint totalBufferSize, int packetId)
        {
            this.PacketID = packetId;
            this.TotalBufferSize = totalBufferSize;
            this.CurentBufferSize = 0;
            this.Buffer = new byte[this.TotalBufferSize];
        }

        public void WriteFragment(byte[] buffer, int indexFragment)
        {
            this.CurentBufferSize = this.CurentBufferSize + (uint)buffer.Length;
            if (this.CurentBufferSize > this.TotalBufferSize)
            {
                this.CurentBufferSize = this.TotalBufferSize;
                return;
            }
            
            uint offset = (uint)indexFragment * 1000;
            for (uint i = 0; i < buffer.Length; ++i)
                Buffer[offset + i] = buffer[i];
        }

        public void Dispose()
        {
            this.Buffer = null;
        }
    }
}