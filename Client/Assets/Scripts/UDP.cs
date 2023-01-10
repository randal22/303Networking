using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5

public class UDP
{
    public UdpClient socket;
    public IPEndPoint endPoint;

    public UDP()
    {
        endPoint = new IPEndPoint(IPAddress.Parse(Client.instance.ip), Client.instance.port);
    }

    public void Connect(int localPort)
    {
        socket = new UdpClient(localPort);

        socket.Connect(endPoint);
        socket.BeginReceive(HandleCallback, null);

        using (Packet pack = new Packet())
        {
            DataSend(pack);
        }
    }

    public void DataSend(Packet pack)
    {
        try
        {
            pack.InsertInt(Client.instance.myId);
            if (socket != null)
            {
                socket.BeginSend(pack.ToArray(), pack.Length(), null, null);
            }
        }
        catch (Exception error)
        {
            Debug.Log($"UDP data send error: {error}");
        }
    }

    private void HandleCallback(IAsyncResult asyncResult)
    {
        try
        {
            byte[] data = socket.EndReceive(asyncResult, ref endPoint);
            socket.BeginReceive(HandleCallback, null);

            if (data.Length < 4)
            {
                Client.instance.Disconnect();
                return;
            }

            PacketHandle(data);
        }
        catch
        {
            Disconnect();
        }
    }

    private void PacketHandle(byte[] data)
    {
        using (Packet packet = new Packet(data))
        {
            int packL = packet.ReadInt();
            data = packet.ReadByteArray(packL);
        }

        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(data))
            {
                int packId = packet.ReadInt();
                Client.packetHandlers[packId](packet);
            }
        });
    }

    private void Disconnect()
    {
        Client.instance.Disconnect();

        endPoint = null;
        socket = null;
    }
}