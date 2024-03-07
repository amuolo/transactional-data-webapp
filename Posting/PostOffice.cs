using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;

namespace Posting;

public class PostOffice : BackgroundService
{
    private PostBox PostBox { get; }

    private HubConnection Connection { get; set; }

    private SemaphoreSlim Semaphore { get; set; } = new(0, 1);

    private bool IsConnected => Connection is not null && Connection.State == HubConnectionState.Connected;

    public PostOffice(PostBox postBox)
    {
        PostBox = postBox;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PostBox.NewMessage += () => Semaphore.Release();

        var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        while(!stoppingToken.IsCancellationRequested)
        {
            await Semaphore.WaitAsync();

            while (!IsConnected)
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                if(Uri.TryCreate(PostBox.PostingUrl, UriKind.Absolute, out var uri)) 
                {
                    Connection = new HubConnectionBuilder().WithUrl(uri).WithAutomaticReconnect().Build();
                    await Connection.StartAsync();
                }
            }

            while (PostBox.Queue.TryDequeue(out var envelope))
            {
                await Connection.SendAsync(Contract.SendMessage, envelope.User, envelope.Message);
            }
        }
    }
}
