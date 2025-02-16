namespace EventDistributor2;

public sealed partial class EventDistributor
{
    public interface IEventSubscriber
    {
        void OnSubscribedEventReceived(EventDistributor.EventWrapper eventWrapper);
    }
}