using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5

public class ClientIn : MonoBehaviour
{
    

    public static void Shake(Packet pack)
    {
        string message = pack.ReadString();
        int localId = pack.ReadInt();
        //confirmation of connection
        Debug.Log($"Server msg {message}");

        Client.instance.myId = localId;
        ClientOut.ShakeReceived();
        //tcp 3 way handshake complete - start udp connection
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    
    //necessary info to spawn a player on the clients world view
    public static void SpawnPlayer(Packet pack)
    {
        int id = pack.ReadInt();
        string username = pack.ReadString();
        Vector3 position = pack.ReadVector3();
        Quaternion rotation = pack.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void DespawnPlayer(Packet pack)
    {
        //despawn the right player
        int targetId = pack.ReadInt();

        GameManager.instance.DespawnPlayer(targetId);
    }




    public static void PlayerPosition(Packet pack)
    {
        int id = pack.ReadInt();
        //Debug.Log(id);
        Vector3 position = pack.ReadVector3();
        //Debug.Log(position);
        Vector3 velocity = pack.ReadVector3();
        //Debug.Log(velocity);

        int s = pack.ReadInt();

        int ms = pack.ReadInt();

        PlayerManager player= GameManager.playersDict[id];
        
        //saving other players info as it comes in, making sure to not mix different players data
       
        player.recentPos.Enqueue(position);
        
        player.secs.Enqueue(s);
        player.msecs.Enqueue(ms);

        if (player.recentPos.Count > 3)
        {
            player.recentPos.Dequeue();
            player.secs.Dequeue();
            player.msecs.Dequeue();
        }
        
        player.transform.position = position;
        
        GameManager.playersDict[id].transform.position = position; 
    }

    public static void PlayerRotation(Packet pack)
    {
        int id = pack.ReadInt();
        Quaternion rotation = pack.ReadQuaternion();

        GameManager.playersDict[id].transform.rotation = rotation;

        //this is to stop unity defaulting the rotation to an invalid one
    }

    
}