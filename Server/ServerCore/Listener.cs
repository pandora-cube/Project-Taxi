using System.Net;
using System.Net.Sockets;

namespace ServerCore;

public class Listener
{
    private Socket _listenSocket = null!;
    private Action<Socket> _onAcceptHandler = null!;

    /// <summary>
    /// 엔드포인트를 통해 소켓 개방
    /// </summary>
    /// <param name="endPoint">접속을 대기할 엔드포인트</param>
    /// <param name="onAcceptHandler">접속이 발생했을 때 실행할 콜백</param>
    /// <remarks>접속 시도 시 Init을 통해 클라이언트 엔드포인트와의 소켓을 개방, 세션 생성</remarks>
    public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
    {
        _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _onAcceptHandler = onAcceptHandler;

        _listenSocket.Bind(endPoint);
        _listenSocket.Listen(10); // 최대 대기열

        // 비동기 접속 수락 시작
        StartAccept();
    }

    private void StartAccept()
    {
        // AcceptAsync를 사용하여 비동기로 접속을 기다립니다.
        Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    Socket clientSocket = await _listenSocket.AcceptAsync();
                    _onAcceptHandler.Invoke(clientSocket);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Accept Error: {e.Message}");
                }
            }
        });
    }
}