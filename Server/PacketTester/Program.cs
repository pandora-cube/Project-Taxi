
using System.Net;
using System.Net.Sockets;
using Protocol;
using Google.Protobuf;

namespace PacketTester;

public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("--- Packet Tester ---");

        // 1. 서버 주소 설정 (localhost:7777)
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7777);

        // 2. 소켓 생성 및 연결
        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            socket.Connect(endPoint);
            Console.WriteLine("Connected to Server!");

            while (true)
            {
                // 3개의 값 입력받기 (posX, posY, posZ) 혹은 'quit' 입력받기
                Console.Write("Enter Position (X Y Z) or 'quit' to exit: ");

                string? input = Console.ReadLine();

                if (string.IsNullOrEmpty(input)) continue;
                if (input.Equals("quit", StringComparison.CurrentCultureIgnoreCase)) break;
                
                string[] parts = input.Split(' ');
                if (parts.Length != 3)
                {
                    Console.WriteLine("Invalid input. Please enter 3 values.");
                    continue;
                }

                if (!float.TryParse(parts[0], out float posX) ||
                    !float.TryParse(parts[1], out float posY) ||
                    !float.TryParse(parts[2], out float posZ))
                {
                    Console.WriteLine("Invalid input. Please enter valid float values.");
                    continue;
                }

                // 3. 테스트할 패킷 생성 (C_Move)
                C_Move movePacket = new C_Move
                {
                    PosX = posX,
                    PosY = posY,
                    PosZ = posZ
                };

                SendPacket(socket, movePacket, PacketID.PktCMove);

                Console.WriteLine($"Sent C_Move Packet (ID: {PacketID.PktCMove}, Size: {movePacket.CalculateSize() + 4})");
                Console.WriteLine($"Position: ({movePacket.PosX}, {movePacket.PosY}, {movePacket.PosZ})");
            }

            socket.Close();
            Console.WriteLine("Connection Closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void SendPacket(Socket socket, IMessage packet, PacketID packetId)
    {
        // 4. 패킷 직렬화 및 헤더 조립 [Size(2)][ID(2)][Payload(N)]
        // Size = PacketID(2) + Payload Length
        byte[] payload = packet.ToByteArray();
        ushort packetIdValue = (ushort)packetId;
        ushort size = (ushort)(payload.Length + 2);

        byte[] sendBuffer = new byte[size + 2];

        // Little Endian으로 헤더 작성
        Array.Copy(BitConverter.GetBytes(size), 0, sendBuffer, 0, 2);
        Array.Copy(BitConverter.GetBytes(packetIdValue), 0, sendBuffer, 2, 2);
        Array.Copy(payload, 0, sendBuffer, 4, payload.Length);

        // 5. 전송
        socket.Send(sendBuffer);
    }
}