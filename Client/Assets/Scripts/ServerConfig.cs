using UnityEngine;

[CreateAssetMenu(fileName = "ServerConfig", menuName = "Scriptable Objects/ServerConfig")]
public class ServerConfig : ScriptableObject
{
    public string ServerIp;
    public int Port;
}
