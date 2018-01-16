using System;

namespace SapphireNetwork
{
    public class NetworkConfiguration
    {
        public INetworkCryptor Cryptor = null;
        public Int32 TimeOut = 15;
        public String ServerIP = "0.0.0.0";
        public Int32 ServerPort = 10015;
        public byte IndeficationByte { get; set; } = 0x00;

        public NetworkConfiguration(byte indeficationByte = 0x00)
        {
            this.IndeficationByte = indeficationByte;
        }
    }
}