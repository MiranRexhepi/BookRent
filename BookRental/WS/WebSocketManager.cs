using System.Net.WebSockets;
using System.Text;

namespace BookRental.WS;
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

    public async Task BroadcastAsync(string message)
    {
        var buffer = Encoding.UTF8.GetBytes(message);

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