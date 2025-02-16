using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EventDistributor2;

/// <summary>
/// This EventDistributor distributes event data to subscribers using the Type of the event data.
/// When the event data is published, it is first pushed to a Queue and then processed by the subscribers asynchronously.
/// 
/// It decouples consumers (subscribers) from producers which makes the architecture more modular and easier to extend and maintain.
/// 
/// The handlers are called asynchronously, so a long lasting handler will not block the next handlers.
/// </summary>
public sealed partial class EventDistributor
{
    private static readonly Lazy<EventDistributor> _instance = new(() => new EventDistributor());
    private EventDistributor() { }
    public static EventDistributor Instance => _instance.Value;

    private readonly ConcurrentDictionary<Type, List<object>> _subscribers = new();
    private readonly ConcurrentQueue<EventWrapper> _eventQueue = new ConcurrentQueue<EventWrapper>();
    private int _publishedEventCount = 0;

    public void Subscribe(Type eventType, object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(eventType);

        _subscribers.AddOrUpdate(eventType,
            _ => new List<object> { subscriber },
            (_, list) =>
            {
                if (!list.Contains(subscriber))
                {
                    list.Add(subscriber);
                }
                return list;
            });
    }

    public void UnSubscribe(Type eventType, object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(eventType);

        if (_subscribers.TryGetValue(eventType, out var subscribers))
        {
            subscribers.Remove(subscriber);
            if (subscribers.Count == 0)
            {
                _subscribers.TryRemove(eventType, out _);
            }
        }
    }

    public void UnSubscribeAll(object subscriber)
    {
        ArgumentNullException.ThrowIfNull(subscriber);

        var eventTypes = _subscribers.Keys.ToList();

        foreach (var eventType in eventTypes)
        {
            if (_subscribers.TryGetValue(eventType, out var subscribers))
            {
                subscribers.Remove(subscriber);
                if (subscribers.Count == 0)
                {
                    _subscribers.TryRemove(eventType, out _);
                }
            }
        }
    }

    public async Task PublishAsync<T>(T eventData) where T : class
    {
        var eventWrapper = new EventWrapper
        {
            EventType = typeof(T),
            EventData = eventData,
            PublishTime = GetCurrentTimestamp()
        };

        _eventQueue.Enqueue(eventWrapper);
        Interlocked.Increment(ref _publishedEventCount);

        await ProcessEventsAsync();
    }

    private async Task ProcessEventsAsync()
    {
        while (_eventQueue.TryDequeue(out var eventWrapper))
        {
            if (_subscribers.TryGetValue(eventWrapper.EventType, out var subscribers))
            {
                foreach (var subscriber in subscribers)
                {
                    if (subscriber is IEventSubscriber eventSubscriber)
                    {
                        await Task.Run(() => eventSubscriber.OnSubscribedEventReceived(eventWrapper));
                        var processingTime = eventWrapper.ElapsedMsSincePublish;
                        Console.WriteLine($"Event of type {eventWrapper.EventType.Name} processed in {processingTime} ms");
                    }
                }
            }
            Interlocked.Decrement(ref _publishedEventCount);
        }
    }

    public int GetPublishedEventCount() => _publishedEventCount;

    /// <summary>
    /// Get the current time-stamp in ticks
    /// </summary>
    /// <returns></returns>
    private static long GetCurrentTimestamp()
    {
        return _stopwatch.ElapsedTicks;
    }

    /// <summary>
    /// Single Stopwatch instance for the entire application
    /// </summary>
    private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
}