using System.Diagnostics;

using System;

namespace VibeSoft.ObjectDistributor;

public class DistributorNotification
{
    public object? MessageObject { get; }
    public string? MessageObjectTypeName { get; }
    public object? Sender { get; }
    public long PublishTimeStamp { get; }
    public double ElapsedMsSincePublish => TicksToMilliseconds(GetCurrentTimestamp() - PublishTimeStamp); 


    /// <summary>
    /// Constructor for DistributorNotification
    /// </summary>
    /// <param name="messageObject">the message which must be distributed</param>
    /// <param name="sender">source of the message; who has sends it (optional), caller of this method should use "this"</param>/// 
    public DistributorNotification(object? messageObject, object? sender = null)
    {
        ArgumentNullException.ThrowIfNull(messageObject);

        MessageObject = messageObject;
        Sender = sender;
        MessageObjectTypeName = MessageObject.GetType().FullName;

        PublishTimeStamp = GetCurrentTimestamp();
    }


    /// <summary>
    /// Get the current time-stamp in ticks
    /// </summary>
    /// <returns></returns>
    private static long GetCurrentTimestamp()
    {
        return _stopwatch.ElapsedTicks;
    }

    /// <summary>
    /// Convert ticks to milliseconds
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns></returns>
    private static double TicksToMilliseconds(long ticks)
    {
        return (double)ticks / Stopwatch.Frequency * 1000;
    }

    /// <summary>
    /// Single Stopwatch instance for the entire application
    /// </summary>
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
}
