using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        internal static List<SapphireType> ListAwakedSapphireTypes = new List<SapphireType>();
        internal static List<SapphireType> ListRemovedSapphireTypes = new List<SapphireType>();
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
                        CallBackObject.SetPoolObject(cbo);
                    }

                    if (ListAwakedSapphireTypes.Count > 0)
                    {
                        for (int i = 0; i < ListAwakedSapphireTypes.Count; i++)
                        {
                            if (Framework.IsWork == false)
                                break;
                            if (ListRemovedSapphireTypes.Contains(ListAwakedSapphireTypes[i]) == false && ListActiveSapphireTypes.Contains(ListAwakedSapphireTypes[i]) == false)
                                ListActiveSapphireTypes.Add(ListAwakedSapphireTypes[i]);
                        }
                        ListAwakedSapphireTypes.Clear();
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
                    
                    if (ListRemovedSapphireTypes.Count > 0)
                    {
                        for (int i = 0; i < ListRemovedSapphireTypes.Count; i++)
                        {
                            if (Framework.IsWork == false)
                                break;
                            if (ListRemovedSapphireTypes.Contains(ListRemovedSapphireTypes[i]) == false && ListActiveSapphireTypes.Contains(ListRemovedSapphireTypes[i]))
                                ListActiveSapphireTypes.Remove(ListRemovedSapphireTypes[i]);
                        }
                        ListRemovedSapphireTypes.Clear();
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