using System.Diagnostics;
using VibeSoft.ObjectDistributor;

namespace ObjectDistributorUsage_Example;

internal class Program
{
    static void Main(string[] args)
    { 
        // Setup. Create the distributor and an example subscriber (can be any class)
        var objectDistributor = new ObjectDistributor();
        var subscriber = new SubscriberExample(objectDistributor);

        // In this example, the subscriber subscribes on 2 message types in its constructor.
        // Now we can distribute messages of these types to the subscriber.

        // Fist create an object with a Message string
        var messageObject1 = new MessageExample1("Hello from 1st MessageExample1!");

        // Then create an object with 2 integers, which sums up to 9, and can be found via its Result method.
        var messageObject2 = new MessageExample2(4, 5);

        // Distribute many objects to the subscriber
        var sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < 100; i++)
        {
            objectDistributor.Distribute(messageObject1);
            objectDistributor.Distribute(messageObject2);
        }
        sw.Stop();
        Console.WriteLine($"Published & Processed 200 events in {sw.ElapsedMilliseconds} ms");
        
        // Dispose the subscriber, which will unsubscribe all its subscribed messages.
        subscriber.Dispose();

        // Distribute again an object with a Message string; but this should not be handled since subscriber has unsubscribed (in its dispose)
        var messageObject3 = new MessageExample1("Hello from 2nd MessageExample1!");
        objectDistributor.Distribute(messageObject3);

    }
}
