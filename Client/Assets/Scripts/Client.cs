using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";//default ip:port values - see ui manager
    public int port = 26950;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;
    public delegate void PacketHandler(Packet pack);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            //already exists
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected");
        }
    }


    public void MakeConnectionToServer()
    {
        InitClientData();
        //check for user inputed info
        if (UIManager.instance.ipField.text != ip)
        {
            Debug.Log("using user inputed value");
            ip =UIManager.instance.ipField.text;
        }
        if (UIManager.instance.portField.text != port.ToString())
        {
            Debug.Log("using user inputed value");
            port = Convert.ToInt32(UIManager.instance.portField.text);
        }
        isConnected = true;
        tcp.Connect();
    }

    

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            //all possible packet types
            { (int)PacketsIn.handshake, ClientIn.Shake },
            { (int)PacketsIn.spawnPlayer, ClientIn.SpawnPlayer },
            { (int)PacketsIn.despawnPlayer, ClientIn.DespawnPlayer },
            { (int)PacketsIn.otherPlayerPosition, ClientIn.PlayerPosition },
            { (int)PacketsIn.otherPlayerRotation, ClientIn.PlayerRotation },
        };
        
    }

    
}