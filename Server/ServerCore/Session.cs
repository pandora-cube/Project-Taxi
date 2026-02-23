using System.Net.Sockets;
using Protocol;

namespace ServerCore;

public class Session
{
    private Socket _socket;

    public Session(Socket socket)
    {
        _socket = socket;
        Console.WriteLine($"Client Connected: {_socket.RemoteEndPoint}");

        StartReceive();
    }

    private async void StartReceive()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (true)
            {
                int receiveLen = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                if (receiveLen <= 0) break;

                // 데이터 처리
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session Error : {e.Message}");
        }
        finally
        {
            _socket.Close();
        }
    }
}