using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace Posting;

public class PostOffice : BackgroundService
{
    private PostBox PostBox { get; set; }

    public HubConnection Connection { get; }

    private SemaphoreSlim Semaphore { get; set; } = new(1, 1);

    private bool IsConnected => Connection is not null && Connection.State == HubConnectionState.Connected;

    public PostOffice(PostBox postBox)
    {
        Connection = new HubConnectionBuilder().WithUrl(Contract.MessageHubAddress).WithAutomaticReconnect().Build();
        PostBox = postBox;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Connection.StartAsync();

        PostBox.NewMessage += SendMessage;
    }

    private void SendMessage()
    {
        Task.Run(async () =>
        {
            if (Semaphore.CurrentCount == 0) return;

            await Semaphore.WaitAsync();

            while(PostBox.Queue.TryDequeue(out var envelope)) 
            {
                await Connection.SendAsync(Contract.SendMessage, envelope.User, envelope.Message);
            }

            Semaphore.Release();
        });
    }
}
