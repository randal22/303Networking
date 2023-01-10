using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class TCP
{
    public TcpClient socketTcp;

    private readonly int id;
    private Packet packetReceived;
    private NetworkStream netStream;
    private byte[] receiveBuffer;

    public TCP(int thisId)
    {
        id = thisId;
    }

    //inits the new client

    public void Connect(TcpClient newClientsocketTcp)
    {
        socketTcp = newClientsocketTcp;
        socketTcp.ReceiveBufferSize = ToClients.bufferSize;
        socketTcp.SendBufferSize = ToClients.bufferSize;

        netStream = socketTcp.GetStream();

        packetReceived = new Packet();
        receiveBuffer = new byte[ToClients.bufferSize];

        netStream.BeginRead(receiveBuffer, 0, ToClients.bufferSize, ReceiveCallback, null);

        ServerOut.Shake(id, "Welcome!");
    }


    public void SendData(Packet packetToSend)
    {
        try
        {
            if (socketTcp != null)
            {
                netStream.BeginWrite(packetToSend.ToArray(), 0, packetToSend.Length(), null, null);
            }
        }
        catch (Exception error)
        {
            Debug.Log($"Error with data on TCP {error}");
        }
    }


    private void ReceiveCallback(IAsyncResult asyncResult)
    {
        try
        {
            int byteLen = netStream.EndRead(asyncResult);
            if (byteLen <= 0)
            {
                Server.clientsDict[id].DisconnectClient();
                return;
            }

            byte[] data = new byte[byteLen];
            Array.Copy(receiveBuffer, data, byteLen);

            packetReceived.Reset(HandleData(data));
            netStream.BeginRead(receiveBuffer, 0, ToClients.bufferSize, ReceiveCallback, null);
        }
        catch (Exception error)
        {
            Debug.Log($"TCP data error: {error}");
            Server.clientsDict[id].DisconnectClient();
        }
    }

    //sort the data before handling
    private bool HandleData(byte[] data)
    {
        int packetLen = 0;

        packetReceived.SetByteArray(data);

        if (packetReceived.UnreadLength() >= 4)
        {

            packetLen = packetReceived.ReadInt();
            if (packetLen <= 0)
            {

                return true;
            }
        }

        while (packetLen > 0 && packetLen <= packetReceived.UnreadLength())
        {

            byte[] bytesFromPacket = packetReceived.ReadBytes(packetLen);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet pack = new Packet(bytesFromPacket))
                {
                    int packId = pack.ReadInt();
                    Server.packetHandlers[packId](id, pack);
                }
            });

            packetLen = 0;
            if (packetReceived.UnreadLength() >= 4)
            {

                packetLen = packetReceived.ReadInt();
                if (packetLen <= 0)
                {

                    return true;
                }
            }
        }

        if (packetLen <= 1)
        {
            return true;
        }

        return false;
    }


    public void Disconnect()
    {
        socketTcp.Close();
        netStream = null;
        packetReceived = null;
        receiveBuffer = null;
        socketTcp = null;
    }
}