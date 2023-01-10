using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
//In order to code this file I watched a youtube tutorial series by Tom Weiland, as c#-unity programming is not in the course material, which can be found here https://www.youtube.com/playlist?list=PLXkn83W0QkfnqsK8I0RAz5AbUxfg3bOQ5

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //dictionary is used to make use of key-value pairing 
    public static Dictionary<int, PlayerManager> playersDict = new Dictionary<int, PlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("already exists");
            Destroy(this);
        }
    }
    //spawn on unity side
    public void SpawnPlayer(int id, string username, Vector3 position, Quaternion rotation)
    {
        GameObject player;
        if (id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab, position, rotation);
        }
        else
        {
            player = Instantiate(playerPrefab, position, rotation);
        }

        player.GetComponent<PlayerManager>().id = id;
        player.GetComponent<PlayerManager>().username = username;
        playersDict.Add(id, player.GetComponent<PlayerManager>());
    }

    public void DespawnPlayer(int id)
    {
        PlayerManager target;
        playersDict.TryGetValue(id, out target);
       
        UnityEngine.Object.Destroy(target.GameObject());
        playersDict.Remove(id);
        
    }

}