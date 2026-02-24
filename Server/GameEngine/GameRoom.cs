// Game Server Info
using ServerCore;
using Protocol;
using Google.Protobuf;

namespace GameEngine;

public class GameRoom : JobSerializer
{
    static int nextPlayerID = 1;
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    /// <summary>
    /// 새 플레이어를 방에 추가하고, 해당 세션에 패킷 핸들러 등록
    /// </summary>
    /// <param name="session"></param>
    public void AddPlayer(Session session)
    {
        Player newPlayer = new Player(nextPlayerID++, session);
        session.BindAction(PacketID.PktCMove, OnMovePacketReceived);

        players.Add(newPlayer.playerID, newPlayer);
    }

    /// <summary>
    /// 플레이어로부터 이동 패킷이 수신되었을 때 실행되는 핸들러
    /// </summary>
    void OnMovePacketReceived(Session session, IMessage packet)
    {
        // only handle C_Move packets
        if (packet is not C_Move movePacket)
        {
            Console.WriteLine("Received non-move packet");
            return;
        }

        Console.WriteLine($"Received move packet: {movePacket.PosX}, {movePacket.PosY}, {movePacket.PosZ}");

        // JobSerializer 큐에 넣어서 순차적으로 처리함
        Enqueue(() =>
        {
            Player? movingPlayer = players.Values.FirstOrDefault(p=> p.session == session);
            if (movingPlayer ==null) return; //해당 플레이어가 존재하지 않을 시 무시

            // 클라이언트에서는 갑자기 위치가 순간이동 되는 문제 발생 가능
            movingPlayer.posX = movePacket.PosX;
            movingPlayer.posY = movePacket.PosY;
            movingPlayer.posZ = movePacket.PosZ;

            S_BroadcastMove broadcastPacket = new S_BroadcastMove
            {
                PlayerId = movingPlayer.playerID,
                PosX = movingPlayer.posX,
                PosY = movingPlayer.posY,
                PosZ = movingPlayer.posZ
            };

            // 나머지 유저들에게 전송
            Broadcast(broadcastPacket, movingPlayer.playerID);

        });
        
    }
        //러스트, 마인크래프트 등은 플레이어 호스트를 사용            
        //메이플 등은 데디케이티드 사용

    public void Broadcast(IMessage packet, int exceptPlayerId = -1)
    {
        foreach (var p in players.Values)
        {
            // C_Move를 호출한 클라이언트는 무시
            if (p.playerID == exceptPlayerId) continue;
            
            // 세션이 만약 비어있다면 무시
            if (p.session == null) return;

            p.session.Send(packet);
        }
    }


}
