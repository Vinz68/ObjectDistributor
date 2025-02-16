using System.Diagnostics;

namespace EventDistributor2;

public sealed partial class EventDistributor
{
    public class EventWrapper
    {
        public required Type EventType { get; set; }
        public required object EventData { get; set; }
        public long PublishTime { get; set; }
        public double ElapsedMsSincePublish => TicksToMilliseconds(GetCurrentTimestamp() - PublishTime);

        /// <summary>
        /// Convert ticks to milliseconds
        /// </summary>
        /// <param name="ticks"></param>
        /// <returns></returns>
        private static double TicksToMilliseconds(long ticks)
        {
            return (double)ticks / Stopwatch.Frequency * 1000;
        }

    }
}