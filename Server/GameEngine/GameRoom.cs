// Game Server Info
using ServerCore;
using Protocol;
using Google.Protobuf;

namespace GameEngine;

public class GameRoom
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
    }
}
