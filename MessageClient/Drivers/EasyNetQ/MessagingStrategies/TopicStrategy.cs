using EasyNetQ;

namespace MessageClient.Drivers.EasyNetQ.MessagingStrategies;

public class TopicStrategy : MessagingStrategies.MessagingStrategy
{
  private readonly string? _clientId;
  private readonly string _topic;
  public TopicStrategy(string? clientId, string topic)
  {
    _clientId = clientId;
    _topic = topic;
  }
  public override void Send<TMessage>(TMessage message, IBus? bus)
  {
    if(bus == null)
      throw new InvalidOperationException("You must call Connect before Send");

    bus.PubSub.Publish(message, _topic);
  }

  public override IDisposable Listen<TMessage>(Action<TMessage> callback, IBus? bus)
  {
    if(bus == null)
      throw new InvalidOperationException("You must call Connect before Listen");
    
    return bus.PubSub.Subscribe(_clientId, callback, x => x.WithTopic(_topic));
  }
}