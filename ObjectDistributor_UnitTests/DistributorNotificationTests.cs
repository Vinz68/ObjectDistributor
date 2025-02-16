using System;
using System.Diagnostics;
using Xunit;


using Microsoft.Extensions.Logging;
using VibeSoft.ObjectDistributor;

namespace MessageDistributorTests;

public class DistributorNotificationTests
{
    // A dummy message type used for test subscriptions.
    private class TestMessage1 
    {
        public TestMessage1()
        {
             Message = "Test Message1";
        }
        public string Message { get; set; }
    }

    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var messageObject = new TestMessage1();
        var sender = this;

        // Act
        var notification = new DistributorNotification(messageObject, this);

        // Assert
        Assert.Equal(messageObject, notification.MessageObject);
        Assert.Equal(sender, notification.Sender);
        Assert.Equal(messageObject.GetType().FullName, notification.MessageObjectTypeName);
        Assert.True(notification.PublishTimeStamp > 0);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenMessageObjectIsNull()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new DistributorNotification(null));
    }

    [Fact]
    public void ElapsedMsSincePublish_ShouldReturnElapsedMilliseconds()
    {
        // Arrange
        var messageObject = new { Content = "Test Message" };
        var notification = new DistributorNotification(messageObject);

        // Act
        var elapsedMs = notification.ElapsedMsSincePublish;

        // Assert
        Assert.True(elapsedMs >= 0);
    }


}
