using System.Net.Sockets;
using Google.Protobuf;
using UnityEngine;
using Protocol;

public class TestSender : MonoBehaviour
{
    private C_Login loginPacket;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loginPacket = new C_Login
        {
            Nickname = "Ozeco"
        };
    }

    void Update()
    {
        Send();
    }

    private void Send()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        IMessage newMsg = loginPacket.Clone();

        try
        {
            socket.Send(newMsg.ToByteArray());
        }
        catch (SocketException e)
        {
            Debug.Log(e.Message);
        }
    }
}
