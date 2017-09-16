using System.Runtime.InteropServices;

namespace SapphireEngine
{
    public class Native
    {
        [DllImport("Kernel32", EntryPoint = "SetConsoleCtrlHandler")]
        internal static extern bool SetSignalHandler(HandlerOnShotdown handler, bool add);
        public delegate void HandlerOnShotdown(int closecode);
        
    }
}