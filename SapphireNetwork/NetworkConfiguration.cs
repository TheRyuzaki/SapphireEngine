using System;

namespace SapphireNetwork
{
    public class NetworkConfiguration
    {
        public Byte IndeficationByte = 0x00;
        public Int32 TimeOut = 15;
        public String ServerIP = "0.0.0.0";
        public Int32 ServerPort = 10015;

        public NetworkConfiguration(Byte indeficationByte = 0x00)
        {
            this.IndeficationByte = indeficationByte;
        }
    }
}