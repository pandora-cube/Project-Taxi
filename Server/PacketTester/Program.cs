using System.Net;
using System.Net.Sockets;
using Protocol;
using Google.Protobuf;
using ServerCore;

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

            // Session 생성 및 설정
            Session session = new Session(socket);

            // S_BroadcastMove 패킷이 오면 콘솔에 출력하도록 핸들러 등록
            session.BindAction(PacketID.PktSBroadcastMove, (s, packet) =>
            {
                if (packet is S_BroadcastMove moveResponse)
                {
                    Console.WriteLine($"\n[RECV] S_BroadcastMove - Player:{moveResponse.PlayerId} Pos:({moveResponse.PosX}, {moveResponse.PosY}, {moveResponse.PosZ})");
                    Console.Write("Enter Position (X Y Z) or 'quit' to exit: "); // 입력 프롬프트 유지
                }
            });

            // 비동기 수신 시작
            session.Start();

            while (true)
            {
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

                // 3. 테스트할 패킷 생성 (C_Move) 및 전송
                C_Move movePacket = new C_Move
                {
                    PosX = posX,
                    PosY = posY,
                    PosZ = posZ
                };

                // Session의 Send 기능을 사용하여 전송
                session.Send(movePacket);
                Console.WriteLine($"[SENT] C_Move Position: ({movePacket.PosX}, {movePacket.PosY}, {movePacket.PosZ})");
            }

            socket.Close();
            Console.WriteLine("Connection Closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}