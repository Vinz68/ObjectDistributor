using System.Diagnostics;

namespace EventDistributor2;
public class MyEvent
{
    public string Message { get; set; }
}

public class AnotherEvent
{
    public int Value { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        // Create a subscriber
        var subscriber = new MySubscriber();

        // Start the subscriber (subscribe to events)
        subscriber.Start();

        var event1 = new MyEvent { Message = "Hello, World-1!" };
        var event2 = new AnotherEvent { Value = 42 };

        // Publish many events
        var sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < 100; i++)
        {
            await EventDistributor.Instance.PublishAsync(event1);
            await EventDistributor.Instance.PublishAsync(event2);
        }
        sw.Stop();
        Console.WriteLine($"Published & Processed 200 events in {sw.ElapsedMilliseconds} ms");

        // Stop the subscriber (unsubscribe from all events)
        subscriber.Stop();

        // Publish events again (subscriber won't receive them)
        await EventDistributor.Instance.PublishAsync(new MyEvent { Message = "This won't be received" });
        await EventDistributor.Instance.PublishAsync(new AnotherEvent { Value = 100 });

        Console.WriteLine($"Published events in queue: {EventDistributor.Instance.GetPublishedEventCount()}");
    }
}