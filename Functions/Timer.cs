using System;

namespace SapphireEngine.Functions
{
    public class Timer : SapphireType
    {
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
                    this.Dispose();
                else
                    this.m_ticked = 0;
            }
        }

        public void Clear() => this.m_ticked = 0f;

        public static Timer SetTimeout(Action _callback, float _timeout)
        {
            Timer timer = Framework.Bootstraper.AddType<Timer>();
            timer.Callback = _callback;
            timer.Interval = _timeout;
            return timer;
        }
        
        public static Timer SetInterval(Action _callback, float _timeout)
        {
            Timer timer = Framework.Bootstraper.AddType<Timer>();
            timer.Callback = _callback;
            timer.Interval = _timeout;
            timer.Repite = true;
            return timer;
        }

    }
}