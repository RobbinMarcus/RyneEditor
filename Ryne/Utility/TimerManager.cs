using System.Collections.Generic;

namespace Ryne.Utility
{
    public static class TimerManager
    {
        private static readonly Dictionary<string, Timer> Timers = new Dictionary<string, Timer>();

        public static void AddTimer(string name)
        {
            Timers.Add(name, new Timer());
        }

        public static Timer GetTimer(string name)
        {
            return Timers[name];
        }

        public static void StartFrame()
        {
            foreach (var timer in Timers)
            {
                timer.Value.StartFrame();
            }
        }

        public static void EndFrame()
        {
            foreach (var timer in Timers)
            {
                timer.Value.EndFrame();
            }
        }

        public static void ClearTimers()
        {
            Timers.Clear();
        }
    }
}
