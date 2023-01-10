using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c# programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class ServerOut
{
    //send to one client
    private static void SendDataTCPTargeted(int targetClient, Packet pack)
    {
        pack.WritePacketLength();
        Server.clientsDict[targetClient].tcp.SendData(pack);
    }

    
    

    
    private static void SendDataTCPAllClients(Packet pack)
    {
        pack.WritePacketLength();
        for (int i = 1; i <= Server.maxPlayerCount; i++)
        {
            Server.clientsDict[i].tcp.SendData(pack);
        }
    }
    
    
   
    //the client who sent the update already knows the info
    private static void SendDataUDPAllOtherClients(int exceptClient, Packet pack)
    {
        pack.WritePacketLength();
        for (int i = 1; i <= Server.maxPlayerCount; i++)
        {
            if (i != exceptClient)
            {
                Server.clientsDict[i].udp.SendData(pack);
            }
        }
    }

        
    public static void Shake(int targetClient, string message)
    {
        using (Packet pack = new Packet((int)PacketsOut.handshake))
        {
            pack.Write(message);
            pack.Write(targetClient);

            SendDataTCPTargeted(targetClient, pack);
        }
    }

    
    public static void SpawnPlayer(int targetClient, Player playerToSpawn)
    {
        using (Packet pack = new Packet((int)PacketsOut.spawnPlayer))
        {
            pack.Write(playerToSpawn.id);
            pack.Write(playerToSpawn.username);
            pack.Write(playerToSpawn.transform.position);
            pack.Write(playerToSpawn.transform.rotation);

            SendDataTCPTargeted(targetClient, pack);
        }
    }

    public static void DespawnPlayer(Player player)
    {
        using (Packet pack = new Packet((int)PacketsOut.despawnPlayer))
        {
            pack.Write(player.id);
            SendDataTCPAllClients(pack);
        }
    }

    //update a player for all other players
    public static void PlayerMoveInfo(Player playerToUpdate)
    {
        using (Packet pack = new Packet((int)PacketsOut.playerPosition))
        {
            pack.Write(playerToUpdate.id);
            pack.Write(playerToUpdate.transform.position);
            pack.Write(playerToUpdate.GetComponent<Rigidbody>().velocity);
            pack.Write(playerToUpdate.s);
            pack.Write(playerToUpdate.ms);
            SendDataUDPAllOtherClients(playerToUpdate.id, pack);
        }
    }

    
    
    
}