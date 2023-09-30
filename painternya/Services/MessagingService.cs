using System;
using System.Collections.Generic;

namespace painternya.Services;

public class MessagingService
{
    public static readonly MessagingService Instance = new MessagingService();

    private readonly List<Action<MessageType, object>> _subscribers = new List<Action<MessageType, object>>();

    private MessagingService() { }

    public void Subscribe(Action<MessageType, object> action)
    {
        _subscribers.Add(action);
    }

    public void Unsubscribe(Action<MessageType, object> action)
    {
        _subscribers.Remove(action);
        Console.WriteLine("Unsubscribed");
    }

    public void Publish(MessageType message, object data = null)
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber(message, data);
        }
    }
}

public enum MessageType
{
    LayerRemoved,
    LayerVisibilityChanged,
    LayerAdded
}
