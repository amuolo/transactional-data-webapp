﻿namespace App.Extensions;

public class Consts
{
    public const string SseConnect = "/sse/connect";

    public const string Url = "https://localhost:7200";

    public const string WebSocketPath = "/ws";

    public const string DataChanged = nameof(DataChanged);

    public const string MessageHubPath = "/messageHub";

    public const string MessageHubAddress = Url + MessageHubPath;

    public const string ReceiveMessage = nameof(ReceiveMessage);

    public const string SendMessage = nameof(SendMessage);
}
