namespace SapphireNetwork
{
    public interface INetworkCryptor
    {
        byte[] Encryption(byte[] buffer);
        byte[] Decryption(byte[] buffer);
    }
}