// Game Server Info
using ServerCore;
using Protocol;

namespace GameEngine;

public class Player
{
    public int playerID;
    public string? nickname;
    public float posX, posY, posZ;
    public float velX, velY, velZ;
    public float rotY;
    
    public Session? session;
}

public class GameRoom
{
    static int nextPlayerID = 1;
    Dictionary<int, Player> players = new Dictionary<int, Player>();

    public void AddPlayer(Session session)
    {
        Player newPlayer = new Player();
        newPlayer.playerID = nextPlayerID++;
        newPlayer.session = session;

        players.Add(newPlayer.playerID, newPlayer);

        // 새 플레이어 정보를 본인에게 전송
        S_EnterGame enterPacket = new S_EnterGame();
        enterPacket.playerID = newPlayer.playerID;
        session.Send(enterPacket);

        // 다른 플레이어들에게 새 플레이어 정보 전송
        S_Spawn spawnPacket = new S_Spawn();
        spawnPacket.playerID = newPlayer.playerID;
        spawnPacket.posX = newPlayer.posX;
        spawnPacket.posY = newPlayer.posY;
        spawnPacket.posZ = newPlayer.posZ;
        spawnPacket.rotY = newPlayer.rotY;

        Broadcast(spawnPacket, exceptPlayerID: newPlayer.playerID);
    }
}
