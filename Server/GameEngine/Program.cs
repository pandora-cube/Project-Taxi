using System.Net;
using ServerCore;
using Protocol;

namespace GameEngine;

class Program
{


    static void Main(string[] args)
    {
        // 1. 엔드포인트 설정    
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 7777);
        
        // 2. 리스너 생성 및 초기화
        Listener listener = new Listener();

        // 접속이 발생했을 때 실행할 콜백 등록
        listener.Init(endPoint, (clientSocket) => {
            // TODO: 첫 소켓 요청이 들어올 시 방 + 플레이어 세션 생성 및 연결
            Session session = new Session(clientSocket);

        });

        Console.WriteLine($"Echo Server is running...");
        Console.WriteLine("Press 'quit' to exit.");

        // 3. 서버 유지
        while (true)
        {
            string? input = Console.ReadLine();
            Console.WriteLine($"Get Input: {input}");
            if (input == "quit") break;
        }
    }
}