using System;

namespace Util.Timer
{
    public class Timer
    {
        public bool sendCallback;
        public float CurrentTime => currentTime;
        public bool IsDone { get; private set; }
        public bool isRunning { get; private set; }

        private float maxTime;
        private float currentTime;
        private System.Action callback;

        public Timer(float maxTime)
        {
            this.maxTime = maxTime;
        }

        public void SetCallback(Action call)
        {
            callback = call;
            sendCallback = true;
        }

        public void Tick(float delta)
        {
            if (!isRunning) {
                return;
            }

            if (!IsDone) {
                currentTime += delta;
                if (currentTime >= maxTime) {
                    IsDone = true;
                    if (sendCallback && callback != null) {
                        callback.Invoke();
                    }
                }
            }
        }

        public void Start() => isRunning = true;
        public void Stop() => isRunning = false;

        public void Reset()
        {
            currentTime = 0;
            IsDone = false;
        }

        public void SetNewTime(float time)
        {
            maxTime = time;
            Reset();
        }

        public void IncreaseTime(float time) => currentTime += time;
        public void DecreaseTime(float time) => currentTime -= time;
    }
}