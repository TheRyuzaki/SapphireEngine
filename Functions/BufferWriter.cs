using System;
using System.IO;
using System.Text;

namespace SapphireEngine.Functions
{
  public class BufferWriter : IDisposable
  {
    private MemoryStream m_memoryStream = new MemoryStream();

    public byte[] Buffer
    {
      get { return this.m_memoryStream.GetBuffer(); }
      set
      {
        this.m_memoryStream.SetLength(0L);
        this.m_memoryStream.Write(value, 0, value.Length);
        this.Position = value.Length;
      }
    }

    public int Length
    {
      get { return this.Buffer.Length; }
    }

    public int Position { get; set; } = 0;

    public BufferWriter()
    {
      this.Start();
    }

    public bool Start()
    {
      this.Buffer = new byte[0];
      return true;
    }

    public void Byte(byte arg)
    {
      this.m_memoryStream.WriteByte(arg);
      ++this.Position;
    }

    public void Boolean(bool arg)
    {
      this.Byte(arg ? (byte) 1 : (byte) 0);
    }

    public void Int16(short arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void UInt16(ushort arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Int32(int arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void UInt32(uint arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Int64(long arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void UInt64(ulong arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Char(char arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Double(double arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Float(float arg)
    {
      byte[] bytes = BitConverter.GetBytes(arg);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Bytes(byte[] arg)
    {
      this.m_memoryStream.Write(arg, 0, arg.Length);
      this.Position += arg.Length;
    }

    public void String(string arg)
    {
      byte[] bytes = Encoding.ASCII.GetBytes(arg);
      this.UInt16((ushort) bytes.Length);
      this.m_memoryStream.Write(bytes, 0, bytes.Length);
      this.Position += bytes.Length;
    }

    public void Dispose()
    {
      this.m_memoryStream?.Dispose();
    }
  }
}