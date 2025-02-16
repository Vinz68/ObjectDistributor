using System;
using Microsoft.Extensions.Logging;

namespace VibeSoft.ObjectDistributor;

public delegate void DistributorNotificationHandler(DistributorNotification distributorNotification);


/// <summary>
/// This ObjectDistributor distributes message objects to subscribers using the fully qualified assembly name of the message class.
/// Distribution of the message objects are done immediately (synchronously) to all subscribers.
/// It decouples consumers (subscribers) from producers which makes the architecture more modular and easier to extend and maintain.
/// 
/// The speed and efficiency depends on the number of subscribers and the total subscriber-handler processing time.
/// So keep subscriber handling time that as short as possible.
/// </summary>
public class ObjectDistributor : IObjectDistributor
{
    public ObjectDistributor(ILogger<ObjectDistributor>? logger = null)
    {
        // Logger can be null ; then no logging; but consider to use a logger, at least during development.
        _logger = logger;

        // for each message type (fully qualified assembly name), we have a list of 0 or more subscribers
        _distributionList = new Dictionary<string, List<DistributorNotificationHandler>>(StringComparer.OrdinalIgnoreCase);
    }


    /// <summary>
    /// Adds a subscriber to the _distributionList of a certain message type.
    /// Only messages with the given messageName will be delivered to the subscriber callback "handler".
    /// </summary>
    /// <param name="type">The message Type. Only messages with this type are delivered to the subscriber.</param>
    /// <param name="handler">Callback Delegate which must be notified on distribution of a certain message type.</param>
    public void Subscribe(Type type, DistributorNotificationHandler handler)
    {
        Subscribe(type.ToString(), handler);
    }


    /// <summary>
    /// Removes a previously subscription of a certain message type.
    /// </summary>
    /// <param name="messageTypeName">The message Type. Only messages with this type are delivered to the subscriber.</param>
    /// <param name="handler">Callback Delegate which must be notified on distribution of a certain message type.</param>
    public void UnSubscribe(Type type, DistributorNotificationHandler handler)
    {
        UnSubscribe(type.ToString(), handler);
    }

    /// <summary>
    /// For given target object, find all handlers and UnSubscribe all previously Subscribed messages.
    /// </summary>
    /// <param name="target">Object which wants to UnSubcribe all its subscribed messages</param>
    public void UnSubscribeAll(object target)
    {
        lock (_syncRoot)
        {
            // Iterate through all subscribed message type names & get list of handlers
            foreach (var msgHandlerList in _distributionList)
            {
                List<DistributorNotificationHandler>? subscribersList = msgHandlerList.Value;

                if (subscribersList is not null)
                {
                    // Find target object in subscriber list and remove it
                    subscribersList.RemoveAll(handler => handler.Target == target);
                }
            }
        }
    }


    /// <summary>
    /// Distributes a Message Notification (with a given type name) to the subscribed receivers.
    /// 
    /// NOTES: 
    ///     The handlers runs in the same thread as where the Distribute method was called from. 
    ///     So you might need thread synchronization in your handler code to handle that.
    /// 
    ///     Do not UnSubscribe in the callback, since this will break the distribution.
    /// 
    /// </summary>
    /// <param name="type">The type name of the message which must be distributed</param>
    /// <param name="message">The message ,wrapped in a notification, which must be delivered to the subscribers.</param>
    public void Distribute(object messageObject, object? sender = null)
    {
        var message = new DistributorNotification(messageObject, sender);

        lock (_syncRoot)
        {
            // Find list of subscribers for given messageType
            _distributionList.TryGetValue(message!.MessageObjectTypeName!, out List<DistributorNotificationHandler>? subscribersList);

            if (subscribersList != null)
            {
                // Notify all subscribers
                foreach (var messageDelegate in subscribersList)
                {
                    try
                    {
                        messageDelegate(message);
                    }
                    catch (Exception ex)
                    {
                        // Do not allow "Exceptions" to stop the message distribution to its subscribers
                        // (do not break the foreach loop)

                        _logger?.LogError("ObjectDistributor failed to Distribute message {MessageName} to handler {Handler}, due to exception: {Exception}.",
                            message!.MessageObjectTypeName!,
                            messageDelegate,
                            ex);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Adds a subscriber to the _distributionList of a certain message type.
    /// Only messages with the given messageName will be delivered to the subscriber callback "handler".
    /// </summary>
    /// <param name="messageTypeName">The messageType name. Only messages with this type name are delivered to the subscriber.</param>/// 
    /// <param name="handler">Callback Delegate which must be notified on distribution of a certain message type.</param>
    private void Subscribe(string messageTypeName, DistributorNotificationHandler handler)
    {
        ArgumentException.ThrowIfNullOrEmpty(messageTypeName);
        ArgumentNullException.ThrowIfNull(handler);

        lock (_syncRoot)
        {
            // Find list of subscribers for given messageType
            _distributionList.TryGetValue(messageTypeName, out List<DistributorNotificationHandler>? subscribersList);

            // If subscribers list does not exists, then create a new subscribers list.
            if (subscribersList is null)
            {
                subscribersList = new List<DistributorNotificationHandler>();
                _distributionList.Add(messageTypeName, subscribersList);
            }
            // Add the new subscriber to the subscribers list.
            subscribersList.Add(handler);
        }
    }

    /// <summary>
    /// Removes a previously subscription of a certain message type.
    /// </summary>
    /// <param name="messageTypeName">The messageType name. Only messages with this type name are delivered to the subscriber.</param>/// 
    /// <param name="handler">Callback Delegate which must be notified on distribution of a certain message type.</param>
    private void UnSubscribe(string messageTypeName, DistributorNotificationHandler handler)
    {
        ArgumentException.ThrowIfNullOrEmpty(messageTypeName);
        ArgumentNullException.ThrowIfNull(handler);

        lock (_syncRoot)
        {
            // Find list of subscribers for given messageType
            _distributionList.TryGetValue(messageTypeName, out List<DistributorNotificationHandler>? subscribersList);

            if (subscribersList is not null)
            {
                subscribersList.Remove(handler);

                // Note: subscriberList can become empty (count=0), but this is OK.
                // Removing the subscriberList has a performance disadvantage
            }
        }
    }

    private readonly ILogger<ObjectDistributor>? _logger;
    private readonly Dictionary<string, List<DistributorNotificationHandler>> _distributionList;
    private static object _syncRoot = new();
}
