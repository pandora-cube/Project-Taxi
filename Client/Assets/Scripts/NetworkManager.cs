using System;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using UnityEngine;
using Protocol;
using Util;

public class NetworkManager : Singleton<NetworkManager>
{
    [SerializeField] ServerConfig _serverConfig;
    Socket _socket;
    private void Awake()
    {
        IPAddress ipAddr = IPAddress.Parse(_serverConfig.ServerIp);
        IPEndPoint endPoint = new IPEndPoint(ipAddr, _serverConfig.Port);

// 2. 소켓 생성 (TCP, IPv4 기준)
        _socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            _socket.Connect(endPoint);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
    }

    public void Send(IMessage packet)
    {
        // 1. 데이터(Body) 직렬화
        byte[] body = packet.ToByteArray();

        // 2. 패킷 ID 추출 (Descriptor 활용)
        // .proto의 메시지 이름(예: "C_Login")을 기반으로 미리 정의한 Enum ID를 찾음
        // 메시지 이름: "C_Login"
        var packetId = packet.GetPacketId();
        
        // 3. 헤더 조립 (총 크기 = 헤더 4바이트 + 바디 길이)
        ushort size = (ushort)(body.Length + 4);
        byte[] finalBuffer = new byte[size];

        // [Size] 넣기 (2바이트)
        Array.Copy(BitConverter.GetBytes(size), 0, finalBuffer, 0, 2);
        // [ID] 넣기 (2바이트)
        Array.Copy(BitConverter.GetBytes((int)packetId), 0, finalBuffer, 2, 2);
        // [Body] 넣기
        Array.Copy(body, 0, finalBuffer, 4, body.Length);

        // 4. 전송 (비동기 권장)
        //_socket.Send(finalBuffer);
    
        // 5. "기다리지 않음" -> 서버가 답장을 주면 OnReceive 등 별도 함수에서 처리됨
    }
}
