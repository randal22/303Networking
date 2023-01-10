using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    public CharacterController controller;
    public int s;
    public int ms;
    
   
    

    

    public void InitPlayer(int thisId, string thisUsername)
    {
        id = thisId;
        username = thisUsername;

    }

    
    

    

    
    public void SetPlayerData(int id,Vector3 pos, Vector3 vel, int s, int ms)
    {
        Server.clientsDict[id].player.transform.position = pos;
        Server.clientsDict[id].player.GetComponent<Rigidbody>().velocity= vel;
        Server.clientsDict[id].player.s = s;
        Server.clientsDict[id].player.ms = ms;
        
    }
}