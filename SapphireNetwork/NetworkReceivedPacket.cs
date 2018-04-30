using System;
using System.Collections.Generic;
using System.Net;

namespace SapphireNetwork
{
    public class NetworkReceivedPacket
    {
        private static Queue<NetworkReceivedPacket> ListPoolObjects = new Queue<NetworkReceivedPacket>();

        public static NetworkReceivedPacket GetPoolObject(IPEndPoint addres, Byte[] buffer)
        {
            NetworkReceivedPacket callBackObject = null;
            if (ListPoolObjects.Count != 0)
            {
                callBackObject = ListPoolObjects.Dequeue();
            }
            else
            {
                callBackObject = new NetworkReceivedPacket();
            }

            callBackObject.Addres = addres;
            callBackObject.Buffer = buffer;

            return callBackObject;
        }

        public static void SetPoolObject(NetworkReceivedPacket obj)
        {
            ListPoolObjects.Enqueue(obj);
        }

        private NetworkReceivedPacket()
        {
        }

        public IPEndPoint Addres;
        public Byte[] Buffer;
    }
}