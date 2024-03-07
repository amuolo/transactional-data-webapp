using System.Collections.Concurrent;

namespace Posting;

internal delegate void MessageHandler();

public class PostBox
{
    internal event MessageHandler? NewMessage;

    internal string? PostingUrl { get; private set; }

    internal ConcurrentQueue<(string User, string Message)> Queue { get; set; } = new();

    public void Enqueue(string postingUrl, string user, string message)
    {
        PostingUrl = postingUrl;
        Queue.Enqueue((user, message));
        OnNewMessage();
    }

    private void OnNewMessage() 
    {
        NewMessage?.Invoke();
    }
}
