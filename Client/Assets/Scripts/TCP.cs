using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class TCP
{
    public TcpClient socket;
    private NetworkStream networkStream;
    private Packet receivedData;
    private byte[] receiveBuffer;

    public void Connect()
    {
        socket = new TcpClient
        {
            ReceiveBufferSize = Client.dataBufferSize,
            SendBufferSize = Client.dataBufferSize
        };

        receiveBuffer = new byte[Client.dataBufferSize];
        socket.BeginConnect(Client.instance.ip, Client.instance.port, ConnectCallback, socket);
    }

    private void ConnectCallback(IAsyncResult asyncResult)
    {
        socket.EndConnect(asyncResult);

        if (!socket.Connected)
        {
            return;
        }

        networkStream = socket.GetStream();

        receivedData = new Packet();

        networkStream.BeginRead(receiveBuffer, 0, Client.dataBufferSize, HandleCallback, null);
    }

    public void DataSend(Packet pack)
    {
        try
        {
            if (socket != null)
            {
                networkStream.BeginWrite(pack.ToArray(), 0, pack.Length(), null, null);
            }
        }
        catch (Exception exception)
        {
            Debug.Log($"TCP data error: {exception}");
        }
    }

    private void HandleCallback(IAsyncResult asyncResult)
    {
        try
        {
            int byteLen = networkStream.EndRead(asyncResult);
            if (byteLen <= 0)
            {
                Client.instance.Disconnect();
                return;
            }

            byte[] data = new byte[byteLen];
            Array.Copy(receiveBuffer, data, byteLen);

            receivedData.Reset(PacketHandle(data));
            networkStream.BeginRead(receiveBuffer, 0, Client.dataBufferSize, HandleCallback, null);
        }
        catch
        {
            Disconnect();
        }
    }

    private bool PacketHandle(byte[] data)
    {
        int packetLen = 0;

        receivedData.SetByteArray(data);

        if (receivedData.UnreadLength() >= 4)
        {
            packetLen = receivedData.ReadInt();
            if (packetLen <= 0)
            {
                return true;
            }
        }

        while (packetLen > 0 && packetLen <= receivedData.UnreadLength())
        {
            byte[] bytesFromPacket = receivedData.ReadByteArray(packetLen);
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet pack = new Packet(bytesFromPacket))
                {
                    int packId = pack.ReadInt();
                    Client.packetHandlers[packId](pack);
                }
            });

            packetLen = 0;
            if (receivedData.UnreadLength() >= 4)
            {
                packetLen = receivedData.ReadInt();
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

    private void Disconnect()
    {
        Client.instance.Disconnect();

        networkStream = null;
        receivedData = null;
        receiveBuffer = null;
        socket = null;
    }
}
