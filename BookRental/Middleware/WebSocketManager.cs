using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace BookRental.Middleware;
public class WebSocketManager
{
    private readonly List<WebSocket> _sockets = [];

    public void AddSocket(WebSocket socket)
    {
        _sockets.Add(socket);
    }

    public void RemoveSocket(WebSocket socket)
    {
        _sockets.Remove(socket);
    }

    public async Task BroadcastAsync(object payload)
    {
        var jsonMessage = JsonSerializer.Serialize(payload);

        var buffer = Encoding.UTF8.GetBytes(jsonMessage);

        foreach (var socket in _sockets.ToList())
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                _sockets.Remove(socket);
            }
        }
    }
}