using System;
using System.Net;

namespace SapphireEngine.Functions
{
    public class IPAddres
    {
        public static uint GetUintFromIP(string ip)
        {
            uint ip_uint = 0;
            if (!string.IsNullOrEmpty(ip))
            {
                byte[] addressBytes = IPAddress.Parse(ip).GetAddressBytes();
                ip_uint = (uint) (addressBytes[0] << 0x18);
                ip_uint += (uint) (addressBytes[1] << 0x10);
                ip_uint += (uint) (addressBytes[2] << 8);
                ip_uint += addressBytes[3];
            }
            return ip_uint;
        }

        public static string GetIPFromUint(uint ip)
        {
            byte[] bytes = BitConverter.GetBytes(ip);
            Array.Reverse(bytes);
            string ipAddress = new IPAddress(bytes).ToString();
            return ipAddress;
        }
    }
}