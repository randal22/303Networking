using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class UDP
{
    public IPEndPoint endPoint;

    private int id;

    public UDP(int thisId)
    {
        id = thisId;
    }


    public void Connect(IPEndPoint newClientEP)
    {
        endPoint = newClientEP;
    }


    public void SendData(Packet packetToSend)
    {
        Server.SendDataUDP(packetToSend, endPoint);
    }


    public void HandleData(Packet dataPacket)
    {
        int packetLen = dataPacket.ReadInt();
        byte[] bytesFromPacket = dataPacket.ReadBytes(packetLen);

        //to make sure unity behaves
        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet pack = new Packet(bytesFromPacket))
            {
                int packId = pack.ReadInt();
                Server.packetHandlers[packId](id, pack);
            }
        });
    }

    //udp connection cleanup
    public void Disconnect()
    {
        endPoint = null;
    }
}


