using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace SapphireEngine.Functions
{
    public class HTTP
    {

        public static void GetRequest(string url, Dictionary<string, string> args, Action<object> callback = null) => GetRequest(url + "?" + EncodeDataArgs(args), callback);
        
        public static void GetRequest(string url, Action<object> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var cbo = new Struct.CallBackObject();
                cbo.Result = new WebClient().DownloadString(url);
                cbo.CallBack = callback;
                if (callback != null)
                    Framework.AddToMainThread(cbo);
            });
        }
        
        public static void PostRequest(string url, Dictionary<string, string> args, Action<object> callback = null) => PostRequest(url, EncodeDataArgs(args), callback);
        
        public static void PostRequest(string url, string queryLine, Action<object> callback = null)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var cbo = new Struct.CallBackObject();
                cbo.Result = new WebClient().UploadString(url, queryLine);
                cbo.CallBack = callback;
                if (callback != null)
                    Framework.AddToMainThread(cbo);
            });
        }

        public static string EncodeDataArgs(Dictionary<string, string> args)
        {
            string result = string.Empty;
            foreach (var row in args)
                result += ((result.Length != 0) ? "&" : "") + row.Key + "=" + row.Value;
            return result;
        }
    }
}