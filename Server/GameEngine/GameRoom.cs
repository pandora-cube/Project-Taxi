// Game Server Info
using ServerCore;
using Protocol;

namespace GameEngine;

public class Player
{
    public int playerID;
    public string? nickname;
    public float posX, posY, posZ;
    
    public Session? session;
}

public class GameRoom
{
    Player p = new();

    p.session = new Session(null!);
}
