using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace Posting;

public class PostOffice : BackgroundService
{
    private PostBox PostBox { get; }

    private HubConnection Connection { get; set; }

    private  IHttpContextAccessor HttpContextAccessor { get; }

    private SemaphoreSlim Semaphore { get; set; } = new(1, 1);

    private HttpContext? HttpContext => HttpContextAccessor.HttpContext;

    private string AppBaseUrl => $"{HttpContext?.Request.Scheme}://{HttpContext?.Request.Host}{HttpContext?.Request.PathBase}";

    private string SignalRUrl => AppBaseUrl + Contract.MessageHubPath;

    private bool IsConnected => Connection is not null && Connection.State == HubConnectionState.Connected;

    public PostOffice(PostBox postBox, IHttpContextAccessor contextAccessor)
    {
        HttpContextAccessor = contextAccessor;
        PostBox = postBox;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PostBox.NewMessage += SendMessage;
    }

    private void SendMessage()
    {
        if (Semaphore.CurrentCount == 0)
            return;

        Task.Run(async () =>
        {
            await Semaphore.WaitAsync();

            if (!IsConnected)
            {
                Connection = new HubConnectionBuilder().WithUrl(SignalRUrl).WithAutomaticReconnect().Build();
                await Connection.StartAsync();
            }

            while (PostBox.Queue.TryDequeue(out var envelope)) 
            {
                await Connection.SendAsync(Contract.SendMessage, envelope.User, envelope.Message);
            }

            Semaphore.Release();
        });
    }
}
