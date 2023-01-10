using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string username;
    //track of recent positions for lerping
    public Queue<Vector3> recentPos;
    //to allow time comparison between packets
    public Queue<int> secs;
    public Queue<int> msecs;
    //public Tuple<int, int> secsTuple;
    //public Tuple<int, int> msecsTuple;
    //public Rigidbody otherPlayerBody;
    private PlayerManager() 
    {

        
        recentPos = new Queue<Vector3>();
        secs = new Queue<int>();
        msecs = new Queue<int>();
        //secsTuple = new Tuple<int,int>(0,0);
       // msecsTuple= new Tuple<int,int>(0,0);
    }

}

