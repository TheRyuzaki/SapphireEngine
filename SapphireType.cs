using System;
using System.Collections.Generic;
using System.Reflection;

namespace SapphireEngine
{
    public class SapphireType : IDisposable
    {
        internal static Dictionary<Type, Dictionary<string, MethodInfo>> ListMethods = new Dictionary<Type, Dictionary<string, MethodInfo>>();
        
        public static float DeltaTime { get; internal set; } = 0f;
        #region [Property] Enable
        public bool Enable
        {
            get => this.m_enable;
            set
            {
                if (this.m_enable != value)
                {
                    switch (value)
                    {
                        case true:
                            FrameworkWorker.ListAwakedSapphireTypes.Add(this);
                            break;
                        case false:
                            FrameworkWorker.ListRemovedSapphireTypes.Add(this);
                            break;
                    }
                    this.m_enable = value;
                }
            }
        }
        #endregion
        
        public ulong TypeUID { get; internal set; } 

        public SapphireType Parent { get; internal set; }
        public List<SapphireType> Children { get; internal set; } = new List<SapphireType>();

        internal Type m_thistype;
        private bool m_enable = false;
        internal bool m_destroyed = false;
        
        ~SapphireType()
        {
            this.Dispose();
        }
        
        
        #region [Method] [Example] AddType

        public object AddType(Type _type, bool _defaultActive = true)
        {
            object instance = null;
            try
            {
                instance = Activator.CreateInstance(_type, true);
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError($"Error to {this.GetType().Name}.AddType<{_type.FullName}>(), Type is not created: " + ex.Message);
                return instance;
            }
            if (instance is SapphireType)
            {
                (instance as SapphireType).Parent = this;
                this.Children.Add(instance as SapphireType);
                (instance as SapphireType).RunAwake(_defaultActive);
                return instance;
            }
            ConsoleSystem.LogError($"Error to {this.GetType().Name}.AddType<{_type.FullName}>(), Type is not have nessed SapphireType");
            return instance;
        }

        public T AddType<T>(bool _defaultActive = true) => (T)this.AddType(typeof(T), _defaultActive);

        #endregion
        
        #region [Method] [Example] GetType

        public T GetType<T>()
        {
            for (int i = 0; i < this.Children.Count; ++i)
                if (this.Children[i] is T)
                    return (T) (object) this.Children[i];
            return default(T);
        }

        #endregion

        #region [Method] [Example] GetTypeInChildren

        public T GetTypeInChildren<T>()
        {
            for (int i = 0; i < this.Children.Count; ++i)
            {
                if (this.Children[i] is T)
                    return (T) (object) this.Children[i];
                if (this.Children[i].Children.Count > 0)
                {
                    var item = this.Children[i].GetTypeInChildren<T>();
                    if (item != null)
                        return item;
                }
            }
            return default(T);
        }

        #endregion

        #region [Method] [Example] GetTypes

        public List<T> GetTypes<T>()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < this.Children.Count; ++i)
                if (this.Children[i] is T)
                    result.Add((T) (object) this.Children[i]);
            return result;
        }

        #endregion

        #region [Method] [Example] GetTypesInChildren

        public List<T> GetTypesInChildren<T>()
        {
            List<T> result = new List<T>();
            for (int i = 0; i < this.Children.Count; ++i)
            {
                result.Add((T) (object) this.Children[i]);
                if (this.Children[i].Children.Count > 0)
                {
                    var items = this.Children[i].GetTypesInChildren<T>();
                    for (int i2 = 0; i2 < items.Count; ++i2)
                        result.Add(items[i2]);
                }
            }
            return result;
        }

        #endregion

        #region [Method] [Example] AsType

        public T AsType<T>()
        {
            if (this is T)
                return (T) (object) this;
            throw new Exception($"Type <{this.GetType().FullName}> is not have parent type <{typeof(T).FullName}>");
            return default(T);
        }

        #endregion

        #region [Method] [Example] IntializationType
        internal void IntializationType()
        {
            if (!ListMethods.ContainsKey(this.m_thistype))
            {
                ListMethods[this.m_thistype] = new Dictionary<string, MethodInfo>();
                MethodInfo[] listmethods = this.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                for (var i = 0; i < listmethods.Length; i++)
                {
                    int variation = 0;
                    if (ListMethods[this.m_thistype].ContainsKey(listmethods[i].Name))
                    {
                        reset:
                        if (ListMethods[this.m_thistype].ContainsKey(listmethods[i].Name + "_" + variation))
                        {
                            variation++;
                            goto reset;
                        }
                        else
                            ListMethods[this.m_thistype].Add(listmethods[i].Name + "_" + variation, listmethods[i]);
                    }
                    else
                        ListMethods[this.m_thistype].Add(listmethods[i].Name, listmethods[i]);
                }
            }
        }
        #endregion
        
        #region [Method] [Example] RunAwake

        internal void RunAwake(bool _defaultActive)
        {
            this.m_thistype = this.GetType();
            this.IntializationType();
            this.TypeUID = FrameworkWorker.TakeNextUID;
            FrameworkWorker.ListSapphireTypes.Add(this.TypeUID, this);
            this.Enable = _defaultActive;
            try
            {
                this.OnAwake();
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError($"Error to {this.GetType().Name}.OnAwake(): " + ex.Message);
            }
        }

        #endregion
        
        
        #region [Method] [Example] RunDestroy

        internal void RunDestroy()
        {
            if (this.m_destroyed)
                return;
            this.m_destroyed = true;
            try
            {
                this.OnDestroy();
            }
            catch (Exception ex)
            {
                ConsoleSystem.LogError($"Error to {this.GetType().Name}.OnDestroy(): " + ex.Message);
            }
        }

        #endregion


        public void SendMessage(string _method, params object[] _args)
        {
            if (ListMethods[this.m_thistype].ContainsKey(_method))
            {
                try
                {
                    ListMethods[this.m_thistype][_method].Invoke(this, _args);
                }
                catch (Exception ex)
                {
                    ConsoleSystem.LogError($"Error to {this.GetType().Name}.SendMessage({_method}): " + ex.Message);
                }
            }
        }
        
        public void BroadcastMessage(string _method, params object[] _args)
        {
            this.SendMessage(_method, _args);
            for (var i = 0; i < this.Children.Count; i++)
                this.Children[i].BroadcastMessage(_method, _args);
        }
        
        public virtual void OnAwake()
        {
            
        }

        public virtual void OnUpdate()
        {
            
        }

        public virtual void OnDestroy()
        {
            
        }
        
        #region [Method] [Override] ToString()

        public override string ToString()
        {
            return $"{this.GetType().Name} ({this.GetType().FullName})";
        }

        #endregion
        
        public void Dispose()
        {
            if (this.TypeUID != 0)
            {
                for (var i = 0; i < this.Children.Count; ++i)
                    this.Children[i].Dispose();
                this.Parent?.Children.Remove(this);
                if (FrameworkWorker.ListSapphireTypes.ContainsKey(this.TypeUID))
                    FrameworkWorker.ListSapphireTypes.Remove(this.TypeUID);
                this.Enable = false;
                this.RunDestroy();
            }
        }
    }
}