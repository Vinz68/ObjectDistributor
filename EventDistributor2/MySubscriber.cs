using static EventDistributor2.EventDistributor;

namespace EventDistributor2;

/// <summary>
/// Just a simple class which subscribes on two different events.
/// It implements the IEventSubscriber interface which is required to subscribe to events.
/// </summary>
public class MySubscriber : IEventSubscriber
{
    public void Start()
    {
        // Subscribe to events
        EventDistributor.Instance.Subscribe(typeof(MyEvent), this);
        EventDistributor.Instance.Subscribe(typeof(AnotherEvent), this);
        Console.WriteLine("MySubscriber started and subscribed to events.");
    }

    public void Stop()
    {
        // Unsubscribe from all events
        EventDistributor.Instance.UnSubscribeAll(this);
        Console.WriteLine("MySubscriber stopped and unsubscribed from all events.");
    }

    public void OnSubscribedEventReceived(EventDistributor.EventWrapper eventWrapper)
    {
        if (eventWrapper.EventType == typeof(MyEvent))
        {
            var myEvent = (MyEvent)eventWrapper.EventData;
            Console.WriteLine($"Received MyEvent: {myEvent.Message}");
        }
        else if (eventWrapper.EventType == typeof(AnotherEvent))
        {
            var anotherEvent = (AnotherEvent)eventWrapper.EventData;
            Console.WriteLine($"Received AnotherEvent: {anotherEvent.Value}");
        }
    }
}
