using System.Net.Sockets;
using System.Security.Cryptography;
using Google.Protobuf;
using Protocol;

namespace ServerCore;

public class Session
{
    private Socket _socket;

    Dictionary<PacketID, Action<IMessage>> packetHandlers = new Dictionary<PacketID, Action<IMessage>>();

    public void BindAction(PacketID packetID, Action<IMessage> action)
    {
        

        packetHandlers.Add(packetID, action);
    }

    void HandlePacket( byte[] buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer, 0);
        PacketID packetID = (PacketID)BitConverter.ToUInt16(buffer, 2);
        
        string className = packetID.ToString().Replace("Pkt", "").Insert(1, "_");
        Type type = Type.GetType($"{className}");
        IMessage packet = ((MessageParser)type.GetProperty("Parser").GetValue(null)).ParseFrom(
            buffer, 4, size - 2
        );

        if (packetHandlers.TryGetValue(packetID, out Action<IMessage>? action))
        {
            action.Invoke(packet);
        }
        else
        {
            Console.WriteLine($"No handler for packet ID: {packetID}");
        }
    }


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

                HandlePacket(buffer);

        
                //=========


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