using System.Net.Sockets;
using System.Security.Cryptography;
using Google.Protobuf;
using Protocol;

namespace ServerCore;

public class Session
{
    #region Static Reflection Caching
    // 리플렉션 캐싱을 위한 딕셔너리
    static readonly Dictionary<PacketID, MessageParser> _parsers = new Dictionary<PacketID, MessageParser>();

    static Session()
    {
        // 정적 생성자에서 모든 PacketID에 대해 Parser를 미리 캐싱
        foreach (PacketID packetID in Enum.GetValues<PacketID>())
        {
            // PktCMove -> C_Move
            string className = packetID.ToString().Replace("Pkt", "");
            if (className.Length > 1)
                className = className.Insert(1, "_");

            Type? type = Type.GetType($"Protocol.{className}, Shared");
            if (type == null) continue;

            var parserProperty = type.GetProperty("Parser");
            if (parserProperty?.GetValue(null) is MessageParser parser)
            {
                _parsers[packetID] = parser;
            }
        }
    }
    #endregion

    private Socket _socket;
    public int PlayerID { get; set; }

    readonly Dictionary<PacketID, Action<Session, IMessage>> packetHandlers = [];

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

        // 캐싱된 Parser가 있는지 확인
        if (!_parsers.TryGetValue(packetID, out MessageParser? parser))
        {
            Console.WriteLine($"No parser found for: {packetID}");
            return;
        }

        // 캐싱된 Parser 사용
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