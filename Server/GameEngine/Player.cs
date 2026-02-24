// Game Server Info
using ServerCore;

namespace GameEngine;

public class Player
{
    public int playerID;
    public string? nickname;
    public float posX, posY, posZ;
    public float velX, velY, velZ;
    public float rotY;

    public Session? session;

    public Player(int id, Session session)
    {
        playerID = id;
        this.session = session;
        posX = posY = posZ = 0;
        velX = velY = velZ = 0;
        rotY = 0;
    }
}
