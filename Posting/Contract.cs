﻿namespace Posting;

public class Contract
{
    public const string SseConnect = "/sse/connect";

    public const string Url = "https://localhost:7200";

    public const string WebSocketPath = "/ws";

    public const string MessageHubPath = "/messageHub";

    public const string MessageHubAddress = Url + MessageHubPath;

    public const string ReceiveMessage = nameof(ReceiveMessage);

    public const string SendMessage = nameof(SendMessage);

    public const string DataChanged = nameof(DataChanged);
}