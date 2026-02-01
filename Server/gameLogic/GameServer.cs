// Game Server Info

using ServerCore;
using Protocol;

namespace GameLogic;

public class Player
{
    public int playerID;
    public string? nickname;
    public List<Session> connects = [];

    public float posX, posY, posZ;
}

public class GameServer
{
    public Player[] players = new Player[4];

    public void RefreshPos(int playerID)
    {

    }
}
