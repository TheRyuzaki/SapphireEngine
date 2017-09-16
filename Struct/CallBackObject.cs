using System;

namespace SapphireEngine.Struct
{
    public struct CallBackObject
    {
        public Action<object> CallBack;
        public object Result;
    }
}