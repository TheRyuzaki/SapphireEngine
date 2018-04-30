using System;
using System.Collections.Generic;

namespace SapphireEngine.Functions
{
    public class Timer : SapphireType
    {
        private static Queue<Timer> PoolTimers = new Queue<Timer>();

        public static Timer GetPoolObject()
        {
            Timer timer = null;
            if (PoolTimers.Count != 0)
                timer = PoolTimers.Dequeue();
            else
                timer = Framework.Bootstraper.AddType<Timer>();
            return timer;
        }

        public static void SetPoolObject(Timer obj)
        {
            obj.Enable = false;
            obj.Callback = null;
            PoolTimers.Enqueue(obj);
        }
        
        
        
        public float Interval = 0f;
        public Action Callback = null;
        public bool Repite = false;
        private float m_ticked = 0f;

        public override void OnUpdate()
        {
            if (this.Interval == 0f)
                return;
            this.m_ticked += DeltaTime;
            if (this.m_ticked >= this.Interval)
            {
                this.Callback();
                if (this.Repite == false)
                    SetPoolObject(this);
                else
                    this.m_ticked = 0;
            }
        }

        public void Clear() => this.m_ticked = 0f;

        public static Timer SetTimeout(Action _callback, float _timeout)
        {
            Timer timer = GetPoolObject();
            timer.Callback = _callback;
            timer.Interval = _timeout;
            return timer;
        }
        
        public static Timer SetInterval(Action _callback, float _timeout)
        {
            Timer timer = GetPoolObject();
            timer.Callback = _callback;
            timer.Interval = _timeout;
            timer.Repite = true;
            return timer;
        }

    }
}