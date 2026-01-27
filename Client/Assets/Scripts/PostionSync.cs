using System;
using System.Net.Sockets;
using Google.Protobuf;
using Protocol;
using UnityEngine;

public class PostionSync : MonoBehaviour
{
    private int _count;
    private void Start()
    {
        _count = 0;
    }
    
    // Update is called once per frame
    void Update()
    {
        _count++;
        if (_count >= 16)
        {
            _count -= 16;
            var packet = new C_Move();
            packet.PosX = transform.position.x;
            packet.PosY = transform.position.y;
            packet.PosZ = transform.position.z;
            Sendpacket(packet);
        }
    }
    
    /// <summary>
    /// send packet to server
    /// </summary>
    /// <param name="message">packet</param>
    void Sendpacket(IMessage message)
    {
        Socket socket;
        Debug.Log(message);
        try
        {
            //socket.Send(message.ToByteArray());
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return;
        }
    }
}
