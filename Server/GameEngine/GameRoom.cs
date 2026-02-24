// Game Server Info
using ServerCore;
using Protocol;
using Google.Protobuf;
using System.Numerics;

namespace GameEngine;

public class GameRoom : JobSerializer
{
    static int nextPlayerID = 1;
    readonly Dictionary<int, Player> players = new Dictionary<int, Player>();

    /// <summary>
    /// 새 플레이어를 방에 추가하고, 해당 세션에 패킷 핸들러 등록
    /// </summary>
    /// <param name="session"></param>
    public void AddPlayer(Session session)
    {
        // 1. 세션에 플레이어 ID 부여 (수신 쓰레드에서 바로 접근하므로 여기서 미리 설정)
        int id = Interlocked.Increment(ref nextPlayerID);
        session.PlayerID = id;

        // 2. 룸의 상태(players 딕셔너리)를 건드리는 작업은 큐에 넣어서 처리
        Enqueue(() => {
            Player newPlayer = new Player(id, session);
            session.BindAction(PacketID.PktCMove, OnMovePacketReceived);
            players.Add(id, newPlayer);
            Console.WriteLine($"Player {id} added to room.");
        });
    }

    /// <summary>
    /// 플레이어로부터 이동 패킷이 수신되었을 때 실행되는 핸들러
    /// </summary>
    void OnMovePacketReceived(Session session, IMessage packet)
    {
        if (packet is not C_Move movePacket) return;

        // 이동 패킷이 오면 바로 로직을 수행하지 않고 큐에 넣습니다.
        Enqueue(() => UpdatePlayerPos(session, movePacket));
    }

    private void UpdatePlayerPos(Session session, C_Move movePacket)
    {
        // Enqueue 안에서 실행되므로 players에 안전하게 접근 가능
        if (players.TryGetValue(session.PlayerID, out Player? player))
        {
            player.postion = new Vector3(movePacket.PosX, movePacket.PosY, movePacket.PosZ);

            // 모든 플레이어에게 전파 (브로드캐스트)
            S_BroadcastMove moveResponse = new S_BroadcastMove
            {
                PlayerId = session.PlayerID,
                PosX = movePacket.PosX,
                PosY = movePacket.PosY,
                PosZ = movePacket.PosZ
            };

            foreach (var p in players.Values)
            {
                p.session?.Send(moveResponse);
            }
        }
    }
}
