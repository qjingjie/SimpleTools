using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Utils
{
    public partial class HighResolutionTimer
    {
        #region Fields

        private bool _stopTimer;
        private Task _runTimer;

        #endregion Fields

        #region Constructor

        public HighResolutionTimer()
        {
        }

        #endregion Constructor

        #region Properties

        public int IntervalMs
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                _stopTimer = false;
                _ = TimeBeginPeriod(1);
                _runTimer = Task.Run(() => RunTimer());
            }
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                _stopTimer = true;
                _runTimer?.Wait();

                _ = TimeEndPeriod(1);
            }
        }

        /// <summary>
        /// Execute timer implementation.
        /// </summary>
        private void RunTimer()
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            long previousTime = stopwatch.ElapsedMilliseconds;

            while (!_stopTimer)
            {
                long currentTime = stopwatch.ElapsedMilliseconds;

                if (currentTime - previousTime >= IntervalMs)
                {
                    previousTime += IntervalMs;
                }

                // Using SpinWait for a more accurate delay
                SpinWait.SpinUntil(() => stopwatch.ElapsedMilliseconds >= previousTime + IntervalMs);
                Elapsed?.Invoke();
            }
        }

        [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        private static extern uint TimeBeginPeriod(uint uMilliseconds);

        [DllImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        private static extern uint TimeEndPeriod(uint uMilliseconds);

        #endregion Methods

        #region Events

        public event Action Elapsed;

        #endregion Events
    }
}