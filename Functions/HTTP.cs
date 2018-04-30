using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace SapphireEngine.Functions
{
    public class HTTP
    {

        public static void GetRequest(string _url, Dictionary<string, string> _args, Action<object> _callback = null) => GetRequest(_url + "?" + EncodeDataArgs(_args), _callback);
        
        public static void GetRequest(string _url, Action<object> _callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var cbo = Struct.CallBackObject.GetPoolObject(null, null);
                cbo.Result = new WebClient().DownloadString(_url);
                cbo.CallBack = _callback;
                if (_callback != null)
                    Framework.RunToMainThread(cbo);
            });
        }
        
        public static void PostRequest(string _url, Dictionary<string, string> _args, Action<object> _callback = null) => PostRequest(_url, EncodeDataArgs(_args), _callback);
        
        public static void PostRequest(string _url, string _queryLine, Action<object> _callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var cbo = Struct.CallBackObject.GetPoolObject(null, null);
                cbo.Result = new WebClient().UploadString(_url, _queryLine);
                cbo.CallBack = _callback;
                if (_callback != null)
                    Framework.RunToMainThread(cbo);
            });
        }

        public static string EncodeDataArgs(Dictionary<string, string> _args)
        {
            string result = string.Empty;
            foreach (var row in _args)
                result += ((result.Length != 0) ? "&" : "") + row.Key + "=" + row.Value;
            return result;
        }
    }
}