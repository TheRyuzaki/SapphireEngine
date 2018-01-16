using System;
using System.Text;
using UnityEngine;

namespace SapphireNetwork
{
    public class NetworkReader : INetworkBuffer
    {
        public NetworkPeer Peer { get; }

        public byte[] Buffer
        {
            get => this.m_buffer;
            set
            {
                this.m_buffer = value;
                this.Position = 1;
            }
        }
        public int Length => this.Buffer.Length;
        public int Position { get; set; } = 0;

        private byte[] m_buffer;

        public NetworkReader(NetworkPeer peer)
        {
            this.Peer = peer;
        }
        
        public byte Byte()
        {
            if (this.Length < this.Position + 1)
                return 0;
            byte result = this.Buffer[this.Position];
            this.Position += 1;
            return result;
        }

        public bool Boolean() => this.Byte() == 1;

        public Int16 Int16()
        {
            if (this.Length < this.Position + 2)
                return 0;
            System.Int16 result = BitConverter.ToInt16(this.Buffer, this.Position);
            this.Position += 2;
            return result;
        }

        public UInt16 UInt16()
        {
            if (this.Length < this.Position + 2)
                return 0;
            System.UInt16 result = BitConverter.ToUInt16(this.Buffer, this.Position);
            this.Position += 2;
            return result;
        }

        public Int32 Int32()
        {
            if (this.Length < this.Position + 4)
                return 0;
            System.Int32 result = BitConverter.ToInt32(this.Buffer, this.Position);
            this.Position += 4;
            return result;
        }

        public UInt32 UInt32()
        {
            if (this.Length < this.Position + 4)
                return 0;
            System.UInt32 result = BitConverter.ToUInt32(this.Buffer, this.Position);
            this.Position += 4;
            return result;
        }

        public Int64 Int64()
        {
            if (this.Length < this.Position + 8)
                return 0;
            System.Int64 result = BitConverter.ToInt64(this.Buffer, this.Position);
            this.Position += 8;
            return result;
        }

        public UInt64 UInt64()
        {
            if (this.Length < this.Position + 8)
                return 0;
            System.UInt64 result = BitConverter.ToUInt64(this.Buffer, this.Position);
            this.Position += 8;
            return result;
        }

        public Char Char()
        {
            if (this.Length < this.Position + 2)
                return '\x0';
            System.Char result = BitConverter.ToChar(this.Buffer, this.Position);
            this.Position += 2;
            return result;
        }

        public Double Double()
        {
            if (this.Length < this.Position + 8)
                return 0;
            System.Double result = BitConverter.ToDouble(this.Buffer, this.Position);
            this.Position += 8;
            return result;
        }

        public Single Float()
        {
            if (this.Length < this.Position + 4)
                return 0;
            System.Single result = BitConverter.ToSingle(this.Buffer, this.Position);
            this.Position += 4;
            return result;
        }

        public Byte[] Bytes(int length)
        {
            if (this.Length < this.Position + length)
                return null;
            byte[] result = new byte[length];
            for (int i = 0; i < length; ++i)
                result[i] = this.Buffer[this.Position + i];
            this.Position += length;
            return result;
        }
        
        public String String()
        {
            ushort length = this.UInt16();
            if (length == 0)
                return string.Empty;
            byte[] buffer = this.Bytes(length);
            return Encoding.ASCII.GetString(buffer);
        }

        public Vector2 Vector2()
        {
            float x = this.Float();
            float y = this.Float();
            return new Vector2(x, y);
        }

        public Vector3 Vector3()
        {
            float x = this.Float();
            float y = this.Float();
            float z = this.Float();
            return new Vector3(x, y, z);
        }

        public Vector4 Vector4()
        {
            float x = this.Float();
            float y = this.Float();
            float z = this.Float();
            float w = this.Float();
            return new Vector4(x, y, z, w);
        }
    }
}