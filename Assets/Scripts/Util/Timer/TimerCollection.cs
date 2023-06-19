using System.Collections.Generic;
using UnityEngine;

namespace Util.Timer
{
    public class TimerCollection : MonoBehaviour
    {
        private static List<Timer> timers = new List<Timer>();
        
        public static Timer Create(float time, bool run = true)
        {
            var t = new Timer(time);
            timers.Add(t);
            if (run) {
                t.Start();
            }

            return t;
        }

        public static Timer Create(float time, bool run, System.Action callback)
        {
            var timer = Create(time, run);
            timer.SetCallback(callback);
            return timer;
        }

        private void OnDestroy()
        {
            timers.Clear();
        }

        public static void RemoveTimer(Timer timer)
        {
            timers.Remove(timer);
        }

        private void Update()
        {
            if (timers.Count == 0) {
                return;
            }

            var delta = Time.deltaTime;
            for (int i = 0; i < timers.Count; i++) {
                timers[i].Tick(delta);
            }
        }
    }
}