using Microsoft.AspNetCore.SignalR;

namespace testing_web;

public class ProductHub: Hub
{
    public override async Task OnConnectedAsync()
        {
            Console.WriteLine(
                $"Client connected: {Context.ConnectionId}"
            );

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(
            Exception? exception)
        {
            Console.WriteLine(
                $"Client disconnected: {Context.ConnectionId}"
            );

            await base.OnDisconnectedAsync(exception);
        }
}
