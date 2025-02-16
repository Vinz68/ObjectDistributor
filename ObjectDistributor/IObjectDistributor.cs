using System;

namespace VibeSoft.ObjectDistributor;

public interface IObjectDistributor
{
    void Distribute(object messageObject, object? sender);
    void Subscribe(Type type, DistributorNotificationHandler handler);
    void UnSubscribe(Type type, DistributorNotificationHandler handler);
    void UnSubscribeAll(object target);
}
