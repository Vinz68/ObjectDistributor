using Microsoft.Extensions.Logging;
using VibeSoft.ObjectDistributor;

namespace MessageDistributorTests;

public class ObjectDistributorTests
{
    // A dummy message type used for test subscriptions.
    private class TestMessage1 { }
    private class TestMessage2 { }

    private ILogger<ObjectDistributor> CreateLogger()
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        return loggerFactory.CreateLogger<ObjectDistributor>();
    }

    [Fact]
    public void SubscribeAndDistribute_CallsSubscriber()
    {
        // Arrange
        bool handlerCalled = false;
        var logger = CreateLogger();
        var distributor = new ObjectDistributor(logger);
        DistributorNotificationHandler handler = notification => handlerCalled = true;

        distributor.Subscribe(typeof(TestMessage1), handler);

        // Act
        distributor.Distribute(new TestMessage1(), sender: null);

        // Assert
        Assert.True(handlerCalled);
    }

    [Fact]
    public void UnSubscribe_RemovesSubscriber()
    {
        // Arrange
        bool handlerCalled = false;
        var distributor = new ObjectDistributor(logger: CreateLogger());
        DistributorNotificationHandler handler = notification => handlerCalled = true;

        distributor.Subscribe(typeof(TestMessage1), handler);
        distributor.UnSubscribe(typeof(TestMessage1), handler);

        // Act
        distributor.Distribute(new TestMessage1(), sender: null);

        // Assert
        Assert.False(handlerCalled);
    }

    [Fact]
    public void UnSubscribeAll_RemovesAllSubscribersFromTarget()
    {
        // Arrange
        int callCount = 0;
        var distributor = new ObjectDistributor(logger: CreateLogger());
        var subscriber = new DummySubscriber(() => callCount++);

        // Using instance method delegate ensures Delegate.Target is set.
        distributor.Subscribe(typeof(TestMessage1), subscriber.Handler);
        distributor.Subscribe(typeof(TestMessage2), subscriber.Handler);

        // Act
        distributor.UnSubscribeAll(subscriber);
        distributor.Distribute(new TestMessage1(), sender: null);

        // Assert
        Assert.Equal(0, callCount);
    }

    [Fact]
    public void MultipleSubscribers_AllCalled()
    {
        // Arrange
        int callCount = 0;
        var distributor = new ObjectDistributor(logger: CreateLogger());
        DistributorNotificationHandler handler1 = notification => callCount++;
        DistributorNotificationHandler handler2 = notification => callCount++;

        distributor.Subscribe(typeof(TestMessage1), handler1);
        distributor.Subscribe(typeof(TestMessage1), handler2);

        // Act
        distributor.Distribute(new TestMessage1(), sender: null);

        // Assert
        Assert.Equal(2, callCount);
    }

    [Fact]
    public void Distribute_ContinuesWhenHandlerThrowsException()
    {
        // Arrange
        int goodCallCount = 0;
        var distributor = new ObjectDistributor(logger: CreateLogger());
        DistributorNotificationHandler goodHandler = notification => goodCallCount++;
        DistributorNotificationHandler badHandler = notification => throw new Exception("Test exception");

        distributor.Subscribe(typeof(TestMessage1), badHandler);
        distributor.Subscribe(typeof(TestMessage1), goodHandler);

        // Act (exception in badHandler is caught internally)
        distributor.Distribute(new TestMessage1(), sender: null);

        // Assert
        Assert.Equal(1, goodCallCount);
    }

    [Fact]
    public void Subscribe_NullHandler_ThrowsArgumentNullException()
    {
        // Arrange
        var distributor = new ObjectDistributor(logger: CreateLogger());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => distributor.Subscribe(typeof(TestMessage1), null!));
    }

    [Fact]
    public void UnSubscribe_NullHandler_ThrowsArgumentNullException()
    {
        // Arrange
        var distributor = new ObjectDistributor(logger: CreateLogger());

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => distributor.UnSubscribe(typeof(TestMessage1), null!));
    }

    // A helper class with an instance method to be used as a delegate; 
    // this ensures the delegate's Target is non-null so that UnSubscribeAll can identify it.
    private class DummySubscriber
    {
        private readonly Action _action;

        public DummySubscriber(Action action)
        {
            _action = action;
        }

        public void Handler(DistributorNotification notification)
        {
            _action();
        }
    }
}
