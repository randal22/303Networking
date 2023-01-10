using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class Server
{
    //set in ui manager after on click event
    public static int Port { get; private set; }
    public static int maxPlayerCount { get; private set; }
    
    public delegate void PacketHandler(int sourceClient, Packet pack);

    public static Dictionary<int, PacketHandler> packetHandlers;
    //another dicrionary of clients
    public static Dictionary<int, ToClients> clientsDict = new Dictionary<int, ToClients>();
    private static TcpListener listenerTCP;
    private static UdpClient listenerUDP;

    
    public static void StartServer(int portNumber, int maxPC)
    {
        
        Port = portNumber;
        maxPlayerCount = maxPC;
        
        InitServerData();

        //tcp listener setup
        listenerTCP = new TcpListener(IPAddress.Any, Port);
        listenerTCP.Start();
        listenerTCP.BeginAcceptTcpClient(HandleTCP, null);
       


        //udp listener setup
        listenerUDP = new UdpClient(Port);
        listenerUDP.BeginReceive(HandleUDP, null);
        //server is setup
        
    }
    private static void InitServerData()
    {
        for (int i = 1; i <= maxPlayerCount; i++)
        {
            clientsDict.Add(i, new ToClients(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)PacketsIn.shakeReceived, ServerIn.ShakeReceived },
            { (int)PacketsIn.playerUpdate, ServerIn.PlayerInfoUpdate },
        };
        
    }
    
    //handle tcp connection request
    private static void HandleTCP(IAsyncResult asyncResult)
    {
        TcpClient client = listenerTCP.EndAcceptTcpClient(asyncResult);
        listenerTCP.BeginAcceptTcpClient(HandleTCP, null);
        
        //check against server max
        for (int i = 1; i <= maxPlayerCount; i++)
        {
            if (clientsDict[i].tcp.socketTcp == null)
            {
                clientsDict[i].tcp.Connect(client);
                return;
            }
        }

        
    }

    //handle udp connection request
    private static void HandleUDP(IAsyncResult asyncResult)
    {
        try
        {
            IPEndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = listenerUDP.EndReceive(asyncResult, ref clientEP);
            listenerUDP.BeginReceive(HandleUDP, null);

            if (data.Length < 4)
            {
                return;
            }

            using (Packet pack = new Packet(data))
            {
                int clientId = pack.ReadInt();

                if (clientId == 0)
                {
                    return;
                }

                if (clientsDict[clientId].udp.endPoint == null)
                {
                    // check that this is a new connection
                    clientsDict[clientId].udp.Connect(clientEP);
                    return;
                }

                if (clientsDict[clientId].udp.endPoint.ToString() == clientEP.ToString())
                {
                    // check for fake/incorrect clientID
                    clientsDict[clientId].udp.HandleData(pack);
                }
            }
        }
        catch (Exception error)
        {
            Debug.Log($"client udp connection lost: {error}");
        }
    }

    
    public static void SendDataUDP(Packet pack, IPEndPoint clientEP)
    {
        try
        {
            if (clientEP != null)
            {
                listenerUDP.BeginSend(pack.ToArray(), pack.Length(), clientEP, null, null);
            }
        }
        catch (Exception error)
        {
           Debug.Log($"Udp error {error}");
        }
    }

    
    

    public static void Stop()
    {
        listenerTCP.Stop();
        listenerUDP.Close();
    }
}