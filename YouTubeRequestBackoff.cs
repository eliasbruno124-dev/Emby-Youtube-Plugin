using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Emby.YouTubePlugin
{
    // A denial pauses new extraction attempts for this server. It is not an
    // invitation to retry through another identity, account or network route.
    internal static class YouTubeRequestBackoff
    {
        private static readonly object Gate = new();
        internal const int MinimumLaunchSpacingMilliseconds = 5000;
        internal const int MaximumStartsPerHour = 120;
        private const long HourMilliseconds = 3600000;
        private static readonly SemaphoreSlim LaunchGate = new(1, 1);
        private static readonly Queue<long> Starts = new();
        private static Func<long> _clockMilliseconds = () => Environment.TickCount64;
        private static Func<TimeSpan, CancellationToken, Task> _delay = Task.Delay;
        private static long _untilMilliseconds;
        private static long _lastDeniedMilliseconds;
        private static long? _lastStartMilliseconds;
        private static int _strikes;

        internal static bool IsBlocked()
        {
            lock (Gate) return _clockMilliseconds() < _untilMilliseconds;
        }

        internal static void Record(bool verificationRequired)
        {
            lock (Gate)
            {
                var now = _clockMilliseconds();
                if (_strikes == 0 || now - _lastDeniedMilliseconds > HourMilliseconds)
                    _strikes = 0;
                _strikes = Math.Min(_strikes + 1, 6);
                var delay = 1000L * Math.Min(1800, (verificationRequired ? 300 : 60) * (1 << (_strikes - 1)));
                _untilMilliseconds = Math.Max(_untilMilliseconds, now + delay);
                _lastDeniedMilliseconds = now;
            }
        }

        // Conservative local policy, not an upstream allowance or protection
        // guarantee. Cache hits and joined flights never call this admission.
        // Serialize the actual launch, not just a future time reservation.
        internal static async Task<bool> StartAsync(Func<bool> start, CancellationToken cancellationToken)
        {
            await LaunchGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                while (true)
                {
                    long wait;
                    lock (Gate)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var now = _clockMilliseconds();
                        if (now < _untilMilliseconds)
                            throw new InvalidOperationException("YouTube request cooldown is active.");
                        while (Starts.Count > 0 && now - Starts.Peek() >= HourMilliseconds) Starts.Dequeue();
                        if (Starts.Count >= MaximumStartsPerHour)
                            throw new InvalidOperationException("YouTube local request budget is exhausted; try later.");
                        wait = _lastStartMilliseconds.HasValue
                            ? Math.Max(0, MinimumLaunchSpacingMilliseconds - (now - _lastStartMilliseconds.Value)) : 0;
                        if (wait == 0)
                        {
                            bool started = start();
                            if (started)
                            {
                                var launched = _clockMilliseconds();
                                Starts.Enqueue(launched);
                                _lastStartMilliseconds = launched;
                            }
                            return started;
                        }
                    }
                    await _delay(TimeSpan.FromMilliseconds(wait), cancellationToken).ConfigureAwait(false);
                    // Recheck rate budget and upstream denial after every wait.
                }
            }
            finally { LaunchGate.Release(); }
        }
    }
}
