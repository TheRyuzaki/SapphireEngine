using System;
using System.Collections.Generic;

namespace SapphireEngine.Struct
{
    public class CallBackObject
    {
        private static Queue<CallBackObject> PoolObjects = new Queue<CallBackObject>();

        public static CallBackObject GetPoolObject(Action<object> callBack, object result)
        {
            CallBackObject callBackObject = null;
            if (PoolObjects.Count != 0)
                callBackObject = PoolObjects.Dequeue();
            else
                callBackObject = new CallBackObject();
            callBackObject.CallBack = callBack;
            callBackObject.Result = result;
            return callBackObject;
        }

        public static void SetPoolObject(CallBackObject obj)
        {
            obj.CallBack = null;
            obj.Result = null;
            PoolObjects.Enqueue(obj);
        }

        private CallBackObject()
        {
            
        }
        
        public Action<object> CallBack;
        public object Result;
    }
}