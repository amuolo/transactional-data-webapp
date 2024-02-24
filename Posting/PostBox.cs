using System.Collections.Concurrent;

namespace Posting;

internal delegate void MessageHandler();

public class PostBox
{
    internal event MessageHandler? NewMessage;

    internal ConcurrentQueue<(string User, string Message)> Queue { get; set; } = new();

    public void Enqueue(string user, string message)
    {
        Queue.Enqueue((user, message));
        OnNewMessage();
    }

    private void OnNewMessage() 
    {
        NewMessage?.Invoke();
    }
}
