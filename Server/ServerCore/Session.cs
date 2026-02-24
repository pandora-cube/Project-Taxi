using System.Net.Sockets;
using System.Security.Cryptography;
using Google.Protobuf;
using Protocol;

namespace ServerCore;

public class Session
{
    private Socket _socket;
    public int PlayerID { get; set; }

    Dictionary<PacketID, Action<Session, IMessage>> packetHandlers = new Dictionary<PacketID, Action<Session, IMessage>>();

    public Session(Socket socket)
    {
        _socket = socket;
        Console.WriteLine($"Client Connected: {_socket.RemoteEndPoint}");
    }

    // 클라이언트로부터 데이터 수신 시작
    public void Start()
    {
        StartReceive();
    }

    /// <summary>
    /// 특정 패킷 ID에 대한 핸들러 등록
    /// </summary>
    public void BindAction(PacketID packetID, Action<Session, IMessage> action)
    {
        packetHandlers.Add(packetID, action);
    }

    /// <summary>
    /// 수신된 패킷을 PacketID에 따라 구분하고, 해당 패킷을 변환하여 등록된 핸들러 실행
    /// </summary>
    void HandlePacket(byte[] buffer)
    {
        ushort size = BitConverter.ToUInt16(buffer, 0);
        PacketID packetID = (PacketID)BitConverter.ToUInt16(buffer, 2);

        // PacketID (PktCMove) -> ClassName (C_Move)
        string className = packetID.ToString().Replace("Pkt", "").Insert(1, "_");

        // Reflection을 이용해 Protocol 네임스페이스에서 해당 클래스 타입 찾기
        Type? type = Type.GetType($"Protocol.{className}, Shared");

        if (type == null)
        {
            Console.WriteLine($"Failed to find type: Protocol.{className}");
            return;
        }

        // Protocol.{className} 클래스의 static Parser 속성에서 MessageParser 인스턴스 가져오기
        var parserProperty = type.GetProperty("Parser");
        if (parserProperty == null)
        {
            Console.WriteLine($"Failed to find Parser for: {className}");
            return;
        }

        // MessageParser를 이용해 패킷 역직렬화
        if (parserProperty.GetValue(null) is not MessageParser parser)
        {
            Console.WriteLine($"Failed to get MessageParser for: {className}");
            return;
        }

        // 헤더 4바이트(Size:2, ID:2)를 제외한 나머지가 데이터 길이
        IMessage packet = parser.ParseFrom(buffer, 4, size - 4);

        // 패킷 ID에 해당하는 핸들러가 등록되어 있으면 실행
        if (packetHandlers.TryGetValue(packetID, out Action<Session, IMessage>? action))
        {
            action.Invoke(this, packet);
        }
        else
        {
            Console.WriteLine($"No handler for packet ID: {packetID}");
        }
    }

    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", "");
        PacketID msgId = Enum.Parse<PacketID>($"Pkt{msgName}");

        ushort size = (ushort)packet.CalculateSize();
        byte[] sendBuffer = new byte[size + 4];

        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

        _socket.Send(sendBuffer);
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