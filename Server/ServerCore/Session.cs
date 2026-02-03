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
                // TODO: 수신된 바이트 데이터를 역직렬화
                // 1. PacketID 파싱 후 해당 값에 따라 Packet 구분 및 역직렬화
                // 2. Session 내 핸들러 메서드 호출
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