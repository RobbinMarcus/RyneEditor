using System.Diagnostics;

namespace Ryne.Utility
{
    public class Timer
    {
        private readonly Stopwatch Stopwatch;

        // Reset the captured time on frame start
        public bool ResetOnFrameStart;

        public double Seconds => Stopwatch.Elapsed.TotalSeconds;
        public long MilliSeconds => Stopwatch.ElapsedMilliseconds;

        public Timer()
        {
            Stopwatch = new Stopwatch();
            ResetOnFrameStart = true;
        }

        public void Start()
        {
            Stopwatch.Start();
        }

        public void Stop()
        {
            Stopwatch.Stop();
        }

        public void Reset()
        {
            Stopwatch.Reset();
        }

        public void StartFrame()
        {
            if (ResetOnFrameStart)
            {
                Stopwatch.Reset();
            }
        }

        public void EndFrame()
        {

        }
    }
}
