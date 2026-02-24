// Game Server Info
using System.Numerics;
using ServerCore;

namespace GameEngine;

public class Player(int id, Session session)
{
    public int playerID = id;
    public string? nickname;
    public Vector3 postion = Vector3.Zero;
    public Vector3 velocity = Vector3.Zero;
    public float rotY = 0;

    public Session? session = session;
}
