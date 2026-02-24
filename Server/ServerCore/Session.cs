using System.Net.Sockets;
using System.Security.Cryptography;
using Google.Protobuf;
using Protocol;

namespace ServerCore;

public class Session
{
    private Socket _socket;

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

        IMessage packet = parser.ParseFrom(buffer, 4, size - 2);

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

    public void Send(IMessage packet)
    {
        // 1. 패킷 ID 추출 (Enum 이름에서 Pkt_ 추출 또는 매핑)
        // 현재 리플렉션 구조상 클래스 이름과 PacketID를 매칭하는 로직이 필요합니다.
        string typeName = packet.Descriptor.Name.Replace("_", ""); // C_Move -> CMove
        PacketID packetID = (PacketID)Enum.Parse(typeof(PacketID), $"Pkt{typeName}");

        // 2. 데이터 직렬화
        byte[] sendBuffer = packet.ToByteArray();
        ushort size = (ushort)(sendBuffer.Length + 4); // 헤더(4) + 데이터

        // 3. 최종 패킷 조립 [Size(2)][PacketID(2)][Data...]
        byte[] fullBuffer = new byte[size];
        Array.Copy(BitConverter.GetBytes(size), 0, fullBuffer, 0, 2);
        Array.Copy(BitConverter.GetBytes((ushort)packetID), 0, fullBuffer, 2, 2);
        Array.Copy(sendBuffer, 0, fullBuffer, 4, sendBuffer.Length);

        // 4. 전송
        try
        {
            _socket.Send(fullBuffer);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Send Error: {e.Message}");
        }
    }


}