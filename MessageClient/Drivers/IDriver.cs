using MessageClient.Drivers.EasyNetQ.MessagingStrategies;

namespace MessageClient.Drivers;

using System;

public interface IDriver<T>
{
    bool Connected();
    void Connect();
    void Disconnect();
    void Listen(Action<T> callback);
    IDisposable Listen<TMessage>(Action<TMessage> callback, MessagingStrategy messagingStrategy);
    void Send(T message);
    void Send<TMessage>(TMessage message, MessagingStrategy messagingStrategy);
}