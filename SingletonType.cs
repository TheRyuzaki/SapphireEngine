using System;

namespace SapphireEngine
{
    public class SingletonType<T>
    {
        #region [Property] [Static] Instance

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = (T) Activator.CreateInstance(typeof(T), true);
                return m_instance;
            }
        }

        #endregion

        #region [Property] [Static] IsInitialized

        public static bool IsInitialized => m_instance != null;

        #endregion

        #region [Method] [Static] Setup

        public static T Setup(T instance) => m_instance = instance;

        #endregion

        #region [Field] [Static] m_instance

        private static T m_instance = default(T);

        #endregion
    }
}