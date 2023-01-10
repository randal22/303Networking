using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5

//keeps the clients on the same player count by telling them when other clients have (dis)connected
public class ToClients
{
    
    

    public int id;
    public TCP tcp;
    public UDP udp;
    public Player player;
    public static int bufferSize = 4096;

    public ToClients(int clientId)
    {
        id = clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }


    public void AddPlayerToGame(string newPlayerName)
    {
        player = NetworkManager.instance.InstaPlayerGameObj();
        player.InitPlayer(id, newPlayerName);

        // Send all players to the new player
        foreach (ToClients client in Server.clientsDict.Values)
        {
            if (client.player != null)
            {
                if (client.id != id)
                {
                    ServerOut.SpawnPlayer(id, client.player);
                }
            }
        }

        // Send the new player to all players
        foreach (ToClients client in Server.clientsDict.Values)
        {
            if (client.player != null)
            {
                ServerOut.SpawnPlayer(client.id, player);
            }
        }
    }



    // dc a client
    public void DisconnectClient()
    {
        
        ThreadManager.ExecuteOnMainThread(() =>
        {
            ServerOut.DespawnPlayer(player);
            UnityEngine.Object.Destroy(player.gameObject);
            player = null;
        });

        tcp.Disconnect();
        udp.Disconnect();
    }
}