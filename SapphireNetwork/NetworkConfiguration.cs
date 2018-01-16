using System;

namespace SapphireNetwork
{
    public class NetworkConfiguration
    {
        public INetworkCryptor Cryptor = null;
        public Int32 TimeOut = 15;
        public String ServerIP = "0.0.0.0";
        public Int32 ServerPort = 10015;
    }
}