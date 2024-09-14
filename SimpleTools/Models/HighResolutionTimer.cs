using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SimpleTools.Models
{
    public partial class HighResolutionTimer
    {
        #region Fields

        private readonly Stopwatch _stopwatch;
        private CancellationTokenSource _cancellationTokenSource;

        #endregion Fields

        #region Constructor

        public HighResolutionTimer()
        {
            _stopwatch = new Stopwatch();
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
        /// Start the timer.
        /// </summary>
        public void Start()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                // Timer is already running
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            // Increase timer resolution to 1ms
            TimeBeginPeriod(1);
            Task.Run(() => RunTimer(_cancellationTokenSource.Token));
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
            _stopwatch.Stop();
            // Restore default timer resolution
            TimeEndPeriod(1);
        }

        /// <summary>
        /// Execute timer implementation.
        /// </summary>
        /// <param name="ct"></param>
        private void RunTimer(CancellationToken ct)
        {
            long intervalTicks = TimeSpan.FromMilliseconds(IntervalMs).Ticks;

            _stopwatch.Start();
            long nextTrigger = _stopwatch.ElapsedTicks + intervalTicks;

            while (!ct.IsCancellationRequested)
            {
                if (_stopwatch.ElapsedTicks >= nextTrigger)
                {
                    Elapsed?.Invoke();
                    nextTrigger += intervalTicks;
                }
            }

            _stopwatch.Stop();
        }

        [LibraryImport("winmm.dll", EntryPoint = "timeBeginPeriod", SetLastError = true)]
        private static partial uint TimeBeginPeriod(uint uMilliseconds);

        [LibraryImport("winmm.dll", EntryPoint = "timeEndPeriod", SetLastError = true)]
        private static partial uint TimeEndPeriod(uint uMilliseconds);

        #endregion Methods

        #region Events

        public event Action Elapsed;

        #endregion Events
    }
}