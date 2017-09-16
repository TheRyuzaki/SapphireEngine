using System;
using System.Collections.Generic;
using System.Threading;
using SapphireEngine.Struct;

namespace SapphireEngine
{
    internal class FrameworkWorker
    {
        internal static ulong TakeNextUID
        {
            get
            {
                m_lastTypeUID = m_lastTypeUID + 1;
                return m_lastTypeUID;
            }
        }
        
        internal static Dictionary<ulong, SapphireType> ListSapphireTypes = new Dictionary<ulong, SapphireType>();
        internal static List<SapphireType> ListActiveSapphireTypes = new List<SapphireType>();
        internal static Queue<SapphireType> ListAwakedSapphireTypes = new Queue<SapphireType>();
        internal static Queue<SapphireType> ListRemovedSapphireTypes = new Queue<SapphireType>();
        internal static Queue<CallBackObject> ListCallBackObjects = new Queue<CallBackObject>();
        
        private static ulong m_lastTypeUID = 0;


        internal static void Initialization()
        {
            DateTime dateTime;
            while (Framework.IsWork)
            {
                dateTime = DateTime.Now;
                try
                {
                    while (ListCallBackObjects.Count > 0)
                    {
                        if (Framework.IsWork == false)
                            break;
                        CallBackObject cbo = ListCallBackObjects.Dequeue();
                        try
                        {
                            cbo.CallBack(cbo.Result);
                        }
                        catch (Exception ex)
                        {
                            ConsoleSystem.LogError($"Error to return Object to MainThread <{cbo.Result.GetType()}>: " + ex.Message);
                        }
                    }
                    
                    while (ListAwakedSapphireTypes.Count > 0)
                    {
                        if (Framework.IsWork == false)
                            break;
                        ListActiveSapphireTypes.Add(ListAwakedSapphireTypes.Dequeue());
                    }
                    
                    for (int i = 0; i < ListActiveSapphireTypes.Count; ++i)
                    {
                        if (Framework.IsWork == false)
                            break;
                        if (ListActiveSapphireTypes[i].Enable == false)
                            continue;
                        try
                        {
                            ListActiveSapphireTypes[i].OnUpdate();
                        }
                        catch (Exception ex)
                        {
                            ConsoleSystem.LogError($"Error to {ListActiveSapphireTypes[i].GetType().Name}.OnUpdate(): " + ex.Message);
                        }
                    }
                    while (ListRemovedSapphireTypes.Count > 0)
                    {
                        if (Framework.IsWork == false)
                            break;
                        ListActiveSapphireTypes.Remove(ListRemovedSapphireTypes.Dequeue());
                    }
                }
                catch (Exception ex)
                {
                    ConsoleSystem.LogError($"Error to frame: " + ex.Message);
                }
                Thread.Sleep(Framework.m_fpsmicrotime);
                SapphireType.DeltaTime = (float)DateTime.Now.Subtract(dateTime).TotalSeconds;
            }
        }
    }
}