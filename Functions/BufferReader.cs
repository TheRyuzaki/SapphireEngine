using System;
using System.Text;

namespace SapphireEngine.Functions
{
    public class BufferReader : IDisposable
  {
    private byte[] m_buffer;

    public byte[] Buffer
    {
      get
      {
        return this.m_buffer;
      }
      set
      {
        this.m_buffer = value;
        this.Position = 0;
      }
    }

    public int Length
    {
      get
      {
        return this.Buffer.Length;
      }
    }

    public int Position { get; set; } = 0;

    public BufferReader(byte[] buffer)
    {
        this.m_buffer = buffer;
    }

    public byte Byte()
    {
      if (this.Length < this.Position + 1)
        return 0;
      byte num = this.Buffer[this.Position];
      ++this.Position;
      return num;
    }

    public bool Boolean()
    {
      return (int) this.Byte() == 1;
    }

    public short Int16()
    {
      if (this.Length < this.Position + 2)
        return 0;
      short int16 = BitConverter.ToInt16(this.Buffer, this.Position);
      this.Position += 2;
      return int16;
    }

    public ushort UInt16()
    {
      if (this.Length < this.Position + 2)
        return 0;
      ushort uint16 = BitConverter.ToUInt16(this.Buffer, this.Position);
      this.Position += 2;
      return uint16;
    }

    public int Int32()
    {
      if (this.Length < this.Position + 4)
        return 0;
      int int32 = BitConverter.ToInt32(this.Buffer, this.Position);
      this.Position += 4;
      return int32;
    }

    public uint UInt32()
    {
      if (this.Length < this.Position + 4)
        return 0;
      uint uint32 = BitConverter.ToUInt32(this.Buffer, this.Position);
      this.Position += 4;
      return uint32;
    }

    public long Int64()
    {
      if (this.Length < this.Position + 8)
        return 0;
      long int64 = BitConverter.ToInt64(this.Buffer, this.Position);
      this.Position += 8;
      return int64;
    }

    public ulong UInt64()
    {
      if (this.Length < this.Position + 8)
        return 0;
      ulong uint64 = BitConverter.ToUInt64(this.Buffer, this.Position);
      this.Position += 8;
      return uint64;
    }

    public char Char()
    {
      if (this.Length < this.Position + 2)
        return char.MinValue;
      char ch = BitConverter.ToChar(this.Buffer, this.Position);
      this.Position += 2;
      return ch;
    }

    public double Double()
    {
      if (this.Length < this.Position + 8)
        return 0.0;
      double num = BitConverter.ToDouble(this.Buffer, this.Position);
      this.Position += 8;
      return num;
    }

    public float Float()
    {
      if (this.Length < this.Position + 4)
        return 0.0f;
      float single = BitConverter.ToSingle(this.Buffer, this.Position);
      this.Position += 4;
      return single;
    }

    public byte[] Bytes(int length)
    {
      if (this.Length < this.Position + length)
        return (byte[]) null;
      byte[] numArray = new byte[length];
      for (int index = 0; index < length; ++index)
        numArray[index] = this.Buffer[this.Position + index];
      this.Position += length;
      return numArray;
    }

    public string String()
    {
      ushort num = this.UInt16();
      if ((int) num == 0)
        return string.Empty;
      return Encoding.ASCII.GetString(this.Bytes((int) num));
    }

    public void Dispose()
    {
      this.m_buffer = null;
    }
  }
}