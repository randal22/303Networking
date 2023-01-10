using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5
public class ServerIn
{
    public static void ShakeReceived(int sourceClient, Packet pack)
    {
        int checkClientId = pack.ReadInt();
        string username = pack.ReadString();

        


        if (sourceClient != checkClientId)
        {
            //ids do not match up
            Debug.Log("ID mismatch!");
        }
        //otherwise
        Server.clientsDict[sourceClient].AddPlayerToGame(username);
    }

    public static void PlayerInfoUpdate(int sourceClient, Packet pack)
    {
        int clientId=pack.ReadInt();
        Vector3 playerPos = pack.ReadVector3();
        Vector3 playerVel = pack.ReadVector3();
        int s=pack.ReadInt();
        int ms=pack.ReadInt();
        Server.clientsDict[sourceClient].player.SetPlayerData(clientId, playerPos, playerVel,s,ms);

        ServerOut.PlayerMoveInfo(Server.clientsDict[clientId].player);
    }
}